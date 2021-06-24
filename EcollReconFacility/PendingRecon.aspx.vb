Imports EcollReconFacility.EcollReconWCF

Public Class PendingRecon
    Inherits System.Web.UI.Page

    Dim userid As String = ""

    Public Class TransResult
        Public TransType As Integer
        Public isSuccessful As Boolean
        Public errMsg As String = ""
    End Class

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If IsNothing(Session("ActiveUserID")) Then

            Response.Redirect("~/Login.aspx")

        Else

            userid = Session("ActiveUserID")

        End If

        If IsPostBack Then

            retainGVValues()

        Else

            loadControls()

            If IsNothing(Session("IsReconSaved")) = False Then

                Page.ClientScript.RegisterClientScriptBlock(Me.GetType(),
                                                            "msgBox",
                                                            "MsgBox('" & Session("ReconMsg") & "', 'Successfully saved');", True)

                Session("IsReconSaved") = Nothing
                Session("ReconMsg") = Nothing

            End If

        End If


    End Sub

    Protected Sub gvCreditLines_RowDeleting(sender As Object, e As GridViewDeleteEventArgs)

        Dim index As Integer = e.RowIndex

        deleteCreditLine(gvCreditLines.Rows(index).Cells(1).Text, index)

        retainGVValues()

    End Sub

    Protected Sub OnSelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)

        loadPendingCreditLines()

    End Sub

    Protected Sub btnModalSave_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim trResult As TransResult = SaveCreditLines()

        Dim message As String = ""

        If trResult.isSuccessful Then

            message = "Credit Lines successfully saved"

        Else

            message = trResult.errMsg

        End If

        Page.ClientScript.RegisterClientScriptBlock(Me.GetType(),
                                                            "msgBox",
                                                            "MsgBox('" & message & "', 'Successfully saved');", True)



        Page.Response.Cache.SetCacheability(HttpCacheability.NoCache)

    End Sub
    Private Sub gvCreditLines_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles gvCreditLines.RowDataBound

        If e.Row.RowType = DataControlRowType.DataRow Then

            Select Case e.Row.Cells(8).Text

                Case "CLOSED"

                    Dim hlink1 As New HyperLink

                    With hlink1
                        .ID = "hlView"
                        .Text = "View"
                        .NavigateUrl = "ReconViewerforClosed.aspx?" &
                            "crid=" & e.Row.Cells(1).Text
                    End With

                    e.Row.Cells(9).Controls.Add(hlink1)

                    e.Row.Cells(10).Text = ""


                Case "PENDING"

                    Dim hlinkRecon As New HyperLink
                    Dim hlinkEdit As New HyperLink
                    Dim hlinkRemove As New HyperLink

                    With hlinkRecon
                        .ID = "hlRecon"
                        .Text = "Reconcile"
                        .NavigateUrl = "ReconPerCreditDate.aspx?" &
                            "crid=" & e.Row.Cells(1).Text
                    End With

                    e.Row.Cells(9).Controls.Add(hlinkRecon)

            End Select

        End If

    End Sub
    Private Sub retainGVValues()

        gvCreditLines.DataSource = gvSource
        gvCreditLines.DataBind()

        checkCreditLineRows()

    End Sub

    Protected Property gvSource As DataTable
        Get

            If IsNothing(ViewState("gvCreditLines")) Then

                Return New DataTable

            Else

                Return CType(ViewState("gvCreditLines"), DataTable)

            End If

        End Get
        Set(value As DataTable)
            ViewState("gvCreditLines") = value
        End Set
    End Property

    Private Sub loadControls()

        txtAmountCredited.Text = 0

        'Modal BANK INSTI DROPDOWN
        Dim dtBankInsti As DataTable = getBankInsti()


        With ddlModalBankInsti
            .DataSource = dtBankInsti
            .DataTextField = dtBankInsti.Columns(1).ToString
            .DataValueField = dtBankInsti.Columns(7).ToString
            .DataBind()
        End With


        'MAIN PAGE BANKINSTI DROPDOWN

        With ddlBankInsti
            .DataSource = dtBankInsti
            .DataTextField = dtBankInsti.Columns(1).ToString
            .DataValueField = dtBankInsti.Columns(7).ToString
            .DataBind()
        End With

        ddlBankInsti.Items.Insert(0, New ListItem With {.Text = "ALL PARTNERS", .Value = "ALL"})

        ddlBankInsti.SelectedIndex = 0

        dtpmodalCreditDate.Text = Today.ToString("MMMM dd, yyyy")
        dtpTransDateFrom.Text = Today.ToString("MMMM dd, yyyy")
        dtpTransDateTo.Text = Today.ToString("MMMM dd, yyyy")

        loadPendingCreditLines()

    End Sub

    Private Function getPendingCreditLines(userid As String, selectedBank As String) As DataTable

        Dim dt As New DataTable

        With dt.Columns
            .Clear()
            .Add("rownumber")
            .Add("creditid")
            .Add("bankinsti")
            .Add("creditdate")
            .Add("transdates")
            .Add("amtcredited")
            .Add("amtonfile")
            .Add("credittype")
            .Add("status")
        End With

        Dim svc As New Service1Client
        Dim dtresult As New IngDTResult
        Dim bankinsticode As String = selectedBank.Split("|")(0)
        Dim rownumber As Integer = 1

        dtresult = svc.IngDataTable("sp_get_pending_credited", {"VAR|" & bankinsticode,
                                    "VAR|" & userid})


        If dtresult.isDataGet Then

            For Each dtRow As DataRow In dtresult.DataSetResult.Tables(0).Rows

                Dim creditLineType As String = ""
                Dim amtCredited As Decimal = CDec(dtRow(4).ToString)
                Dim amtTicketed As Decimal = CDec(dtRow(5).ToString)
                Dim transDates As String = CDate(dtRow(2).ToString).ToString("MMMM dd, yyyy") &
                " - " & CDate(dtRow(3).ToString).ToString("MMMM dd, yyyy")

                If amtCredited = amtTicketed Then

                    creditLineType = "BALANCED"

                Else

                    If amtCredited > amtTicketed Then

                        creditLineType = "OVER REMITTANCE"

                    End If

                    If amtCredited < amtTicketed Then

                        creditLineType = "UNDER REMITTANCE"

                    End If


                End If

                dt.Rows.Add({
                    rownumber,
                    dtRow(0).ToString,
                    dtRow(7).ToString,
                    CDate(dtRow(1).ToString).ToString("MMMM dd, yyyy"),
                    transDates,
                    CDec(dtRow(4).ToString).ToString("#,###,##0.00"),
                    CDec(dtRow(5).ToString).ToString("#,###,##0.00"),
                    creditLineType,
                    dtRow(6).ToString
                            })

                rownumber = rownumber + 1

            Next

        End If

        Return dt

    End Function

    Private Sub checkCreditLineRows()

        If gvCreditLines.Rows.Count > 0 Then

            lbltotalAmtPerCreditPeriod.ForeColor = Drawing.Color.Black
            Dim creditDateFrom As String = gvCreditLines.Rows(0).Cells(2).Text
            Dim creditDateTo As String = gvCreditLines.Rows(gvCreditLines.Rows.Count - 1).Cells(2).Text

            Dim totalAmt As Decimal = 0.0

            For Each gvrow As GridViewRow In gvCreditLines.Rows

                totalAmt = totalAmt + CDec(gvrow.Cells(5).Text)

            Next

            lbltotalAmtPerCreditPeriod.Text = ""

        Else

            lbltotalAmtPerCreditPeriod.ForeColor = Drawing.Color.Red
            lbltotalAmtPerCreditPeriod.Text = "No Pending Credit Lines Available"

        End If


    End Sub

    Private Function SaveCreditLines() As TransResult

        Dim isSaved As Boolean = False

        Dim trResult As New TransResult

        Try

            Dim bankinsticode As String = ddlModalBankInsti.SelectedValue.Split("|")(0)
            Dim ecolltype As String = ddlModalBankInsti.SelectedValue.Split("|")(7)

            Dim modalCreditDate As String = CDate(dtpmodalCreditDate.Text).ToString("MM/dd/yyyy")
            Dim transDateFrom As String = CDate(dtpTransDateFrom.Text).ToString("MM/dd/yyyy")
            Dim transDateTo As String = CDate(dtpTransDateTo.Text).ToString("MM/dd/yyyy")

            Dim creditID As String = GetCreditID(bankinsticode, modalCreditDate)

            Dim strParams() As String = {
                "VAR|" & creditID,
                "VAR|" & transDateFrom,
                "VAR|" & transDateTo,
                "VAR|" & bankinsticode,
                "VAR|" & modalCreditDate,
                "VAR|" & txtAmountCredited.Text.Trim(","),
                "VAR|" & userid,
                "VAR|" & ecolltype
                }

            Dim svc As New Service1Client
            Dim dtresult As New IngDTResult

            dtresult = svc.IngDataTable("sp_ins_credit_lines", strParams)

            If dtresult.isDataGet Then

                gvSource = getPendingCreditLines(userid, ddlBankInsti.SelectedValue)

                retainGVValues()

                isSaved = True

                With trResult
                    .isSuccessful = isSaved
                    .TransType = 1
                End With


            Else

                isSaved = False

                With trResult
                    .isSuccessful = isSaved
                    .TransType = 1
                    .errMsg = dtresult.DTErrorMsg
                End With


            End If

        Catch ingEx As Ingres.Client.IngresException

            With trResult
                .isSuccessful = False
                .TransType = 1
                .errMsg = ingEx.Message
            End With

        Catch ex As Exception

            With trResult
                .isSuccessful = False
                .TransType = 1
                .errMsg = ex.Message
            End With

        End Try

        Return trResult
    End Function

    Private Function GetCreditID(bankinsticode As String, creditDate As String) As String

        Dim creditID As String = bankinsticode &
            CDate(creditDate).Year &
            CDate(creditDate).DayOfYear.ToString.PadLeft(3, "0") & "001"

        Dim dtresult As New IngDTResult
        Dim svc As New Service1Client

        Dim strParams As String() = {
            "VAR|" & bankinsticode,
            "VAR|" & creditDate
            }

        dtresult = svc.IngDataTable("sp_get_max_creditid", strParams)

        If dtresult.isDataGet Then

            For Each dtRow As DataRow In dtresult.DataSetResult.Tables(0).Rows

                If Trim(dtRow(0).ToString) = "" Then

                    Exit For

                Else

                    creditID = Val(dtRow(0).ToString) + 1


                End If

            Next

        End If

        Return creditID

    End Function

    Private Function getBankInsti() As DataTable

        Dim dt As New DataTable

        With dt.Columns
            .Add("bankinsticode")
            .Add("bankinsti")
            .Add("depositorybank")
            .Add("contactperson")
            .Add("accountno")
            .Add("contactno")
            .Add("emailadd")
            .Add("longstringinfo")
        End With

        Dim bankcodes As String() = Session("ActiveUserBankCodes").ToString.Split("|")

        For Each bankcode As String In bankcodes

            Dim svc As New Service1Client
            Dim dtresult As New IngDTResult

            dtresult = svc.IngDataTable("sp_get_bankinsti_info", {"VAR|" & bankcode})

            If dtresult.isDataGet Then

                For Each dtrow As DataRow In dtresult.DataSetResult.Tables(0).Rows

                    Dim longstringinfo As String = dtrow(0).ToString & "|" &
                    dtrow(2).ToString & "|" &
                    dtrow(3).ToString & "|" &
                    dtrow(4).ToString & "|" &
                    dtrow(5).ToString & "|" &
                    dtrow(6).ToString & "|" &
                    dtrow(7).ToString & "|" &
                    dtrow(8).ToString

                    dt.Rows.Add({
                                dtrow(0).ToString,
                                dtrow(1).ToString,
                                dtrow(2).ToString,
                                dtrow(3).ToString,
                                dtrow(4).ToString,
                                dtrow(5).ToString,
                                dtrow(6).ToString,
                                longstringinfo
                                })

                Next

            End If


        Next

        Return dt
    End Function

    Private Function deleteCreditLine(creditid As String, rowIndex As Integer) As Boolean

        Dim dtresult As New IngDTResult
        Dim svc As New Service1Client

        dtresult = svc.IngDataTable("sp_del_credit_lines", {"VAR|" & creditid})

        If dtresult.isDataGet Then

            gvSource.Rows.RemoveAt(rowIndex)

        End If

        Return True
    End Function

    Private Sub loadPendingCreditLines()

        'LOAD PENDING CREDIT LINES
        Dim dt As DataTable = getPendingCreditLines(userid, ddlBankInsti.SelectedValue)

        gvCreditLines.DataSource = dt
        gvCreditLines.DataBind()

        gvSource = dt

        checkCreditLineRows()


    End Sub
End Class