Imports EcollReconFacility.EcollReconWCF

Public Class SearchRecon
    Inherits System.Web.UI.Page

    Dim userid As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If IsNothing(Session("ActiveUserID")) Then

            Response.Redirect("~/Login.aspx")

        Else

            userid = Session("ActiveUserID")

        End If

        If IsPostBack Then

        Else

            loadControls()

        End If

    End Sub

    Protected Sub gvCreditLines_RowDeleting(sender As Object, e As GridViewDeleteEventArgs)

        Dim index As Integer = e.RowIndex

        deleteCreditLine(gvReconBackLog.Rows(index).Cells(1).Text, index)

        retainGVValues()


    End Sub

    Protected Sub btnSearchTickets_Click(sender As Object, e As EventArgs) Handles btnSearchTickets.Click

        'lblRMResult.Text = ""

        Dim dt As New DataTable

        dt = getRMCreditLines(ddlBankInsti.SelectedValue.Split("|")(0),
                                             ddlReconYear.SelectedValue,
                                             ddlReconMonth.SelectedValue,
                                             ddlReconStatus.SelectedValue)


        Session("ActiveBankInstiName") = ddlBankInsti.SelectedValue.Split("|")(1)

        gvReconBackLog.DataSource = dt
        gvReconBackLog.DataBind()

        If gvReconBackLog.Rows.Count = 0 Then

            Page.ClientScript.RegisterClientScriptBlock(Me.GetType(),
                            "msgBox",
                            "MsgBox('No record found', 'Result');", True)

            Page.Response.Cache.SetCacheability(HttpCacheability.NoCache)

        End If

        gvRMSource = dt

    End Sub

    Private Sub gvReconBackLog_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles gvReconBackLog.RowDataBound

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

                    Session("ClosedReconBackPage") = "~/SearchRecon.aspx"


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

    Private Sub loadControls()

        Dim dtBankInsti As DataTable = getBankInsti()

        With ddlBankInsti
            .DataSource = dtBankInsti
            .DataTextField = dtBankInsti.Columns(1).ToString
            .DataValueField = dtBankInsti.Columns(7).ToString
            .DataBind()
        End With

        ddlBankInsti.SelectedIndex = 0


        'RECON TAB PAGE

        'lblRMResult.Text = ""

        gvReconBackLog.DataSource = New DataTable
        gvReconBackLog.DataBind()

        With ddlReconYear

            Dim yr As String = Now.Year

            Dim lstItem As New ListItem With {.Text = yr, .Value = yr}

            .Items.Add(lstItem)

            For x As Integer = 3 To 1 Step -1

                yr = yr - 1

                .Items.Add(yr)

            Next

        End With

    End Sub

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
                        dtrow(1).ToString & "|" &
                        dtrow(2).ToString & "|" &
                        dtrow(3).ToString & "|" &
                        dtrow(4).ToString & "|" &
                        dtrow(5).ToString & "|" &
                        dtrow(6).ToString

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

    Private Sub retainGVValues()

        gvReconBackLog.DataSource = gvRMSource
        gvReconBackLog.DataBind()

        'gvSource = New DataTable
        'gvRMSource = New DataTable

    End Sub

    Protected Property gvRMSource As DataTable
        Get

            If IsNothing(ViewState("gvRMCreditLines")) Then

                Return New DataTable

            Else

                Return CType(ViewState("gvRMCreditLines"), DataTable)

            End If

        End Get
        Set(value As DataTable)
            ViewState("gvRMCreditLines") = value
        End Set
    End Property

    Private Function deleteCreditLine(creditid As String, rowIndex As Integer) As Boolean

        Dim dtresult As New IngDTResult
        Dim svc As New Service1Client

        dtresult = svc.IngDataTable("sp_del_credit_lines", {"VAR|" & creditid})

        If dtresult.isDataGet Then

            gvRMSource.Rows.RemoveAt(rowIndex)

        End If

        Return True
    End Function

    Public Function getRMCreditLines(bankinsticode As String, creditYear As String, creditMonth As String, status As String) As DataTable

        Dim dt As New DataTable

        With dt.Columns
            .Add("rownumber")
            .Add("creditid")
            .Add("creditdate")
            .Add("transdatefrom")
            .Add("transdateto")
            .Add("amtcredited")
            .Add("amtonfile")
            .Add("credittype")
            .Add("status")
        End With

        Dim dtresult As New IngDTResult
        Dim svc As New Service1Client

        Dim rownumber As Integer = 1

        Dim strParam As String() = {
            "VAR|" & bankinsticode,
            "VAR|" & creditYear,
            "VAR|" & creditMonth,
            "VAR|" & status
        }

        dtresult = svc.IngDataTable("sp_get_creditlines_bydate", strParam)

        If dtresult.isDataGet Then

            For Each dtRow As DataRow In dtresult.DataSetResult.Tables(0).Rows

                Dim creditLineType As String = ""
                Dim amtCredited As Decimal = CDec(dtRow(4).ToString)
                Dim amtTicketed As Decimal = CDec(dtRow(5).ToString)

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
                    CDate(dtRow(1).ToString).ToString("MMMM dd, yyyy"),
                    CDate(dtRow(2).ToString).ToString("MMMM dd, yyyy"),
                    CDate(dtRow(3).ToString).ToString("MMMM dd, yyyy"),
                    CDec(dtRow(4).ToString).ToString("#,###,##0.00"),
                     CDec(dtRow(5).ToString).ToString("#,###,##0.00"),
                     creditLineType,
                     dtRow(6).ToString})

                rownumber = rownumber + 1

            Next

        End If

        Return dt
    End Function

End Class