Imports EcollReconFacility.EcollReconWCF

Public Class UCMonitoring
    Inherits System.Web.UI.Page

    Dim userid As String = ""

    Public Class TransResult
        Public TransType As Integer
        Public isSuccessful As Boolean = False
        Public resultMsg As String = ""
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

            If IsNothing(Session("ReconSaveStatus")) = False And Session("ReconSaveStatus") = True Then

                Page.ClientScript.RegisterClientScriptBlock(Me.GetType(),
                                                            "msgBox",
                                                            "MsgBox('" & Session("ReconSaveMsg") & "', 'Successfully saved');", True)


                Session("ReconSaveStatus") = Nothing
                Session("ReconSaveMsg") = Nothing

            End If

        End If

    End Sub
    Protected Sub gvUC_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles gvUC.PageIndexChanging

        gvUC.PageIndex = e.NewPageIndex
        loadControls()

    End Sub

    Protected Sub OnSelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)

        loadUCAR()

        Select Case ddlReconType.SelectedValue
            Case "UC"
                gvUC.HeaderRow.Cells(4).Text = "UC Amount"
            Case "AR"
                gvUC.HeaderRow.Cells(4).Text = "AR Amount"
        End Select

    End Sub

    Private Sub btnModalSave_Click(sender As Object, e As EventArgs) Handles btnModalSave.Click

        Dim tresult As TransResult = isSaved(txtCreditID.Value,
                                             txtUCARNo.Value)

        ClientScript.RegisterClientScriptBlock(Me.GetType(),
                                            "msgBox",
                                            "MsgBox('" & tresult.resultMsg & "', 'Successfully saved');", True)

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

        With ddlReconYear

            Dim yr As String = Now.Year

            Dim lstItem As New ListItem With {.Text = yr, .Value = yr}

            .Items.Add(lstItem)

            For x As Integer = 3 To 1 Step -1

                yr = yr - 1

                .Items.Add(yr)

            Next

        End With

        ddlReconYear.SelectedIndex = 0
        ddlReconMonth.SelectedIndex = (Now.Month - 1)


        loadUCAR()

    End Sub

    Private Function getUCAR(bankinsticode As String, reconType As String, status As String,
                             reconMonth As String, reconYear As String) As DataTable

        Dim dt As New DataTable

        With dt.Columns
            .Clear()
            '.Add("rownumber")
            '.Add("creditid")
            '.Add("reconno")
            '.Add("creditdate")
            '.Add("bankinsti")
            '.Add("vardate")
            '.Add("amountonfile")
            '.Add("varamount")
            '.Add("status")

            .Add("rownumber")
            .Add("creditid")
            .Add("reconno")
            .Add("vardate")
            .Add("varamount")
            .Add("status")

        End With

        Dim rownumber As Integer = 1

        Dim dtresult As New IngDTResult
        Dim svc As New Service1Client

        Dim cmdText As String = ""

        Select Case reconType

            Case "AR"

                cmdText = "sp_sel_credit_ar"

            Case "UC"

                cmdText = "sp_sel_credit_uc"

        End Select

        dtresult = svc.IngDataTable(cmdText,
                                    {"VAR|" & userid,
                                    "VAR|" & bankinsticode,
                                    "VAR|" & status,
                                    "VAR|" & reconMonth,
                                    "VAR|" & reconYear})

        If dtresult.isDataGet Then

            For Each dtRow As DataRow In dtresult.DataSetResult.Tables(0).Rows


                dt.Rows.Add({
                                rownumber,
                                dtRow(0).ToString,
                                dtRow(1).ToString,
                                CDate(dtRow(4).ToString).ToString("MMMM dd, yyyy"),
                                CDec(dtRow(6).ToString).ToString("#,###,##0.00"),
                                dtRow(7).ToString
                                })

                rownumber = rownumber + 1

            Next

        End If

        Return dt

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

    Private Sub loadUCAR()

        Dim dt As DataTable = getUCAR(ddlBankInsti.SelectedValue,
                                      ddlReconType.SelectedValue,
                                      ddlReconStatus.SelectedValue,
                                      ddlReconMonth.SelectedValue,
                                      ddlReconYear.SelectedValue)

        gvUC.DataSource = dt
        gvUC.DataBind()

        gvSource = dt

    End Sub

    Protected Property gvSource As DataTable
        Get

            If IsNothing(ViewState("gvUCAR")) Then

                Return New DataTable

            Else

                Return CType(ViewState("gvUCAR"), DataTable)

            End If

        End Get
        Set(value As DataTable)
            ViewState("gvUCAR") = value
        End Set
    End Property

    Private Sub retainGVValues()

        gvUC.DataSource = gvSource
        gvUC.DataBind()

    End Sub


    Private Function isSaved(creditid As String, reconno As String) As TransResult

        Dim svc As New Service1Client
        Dim dtresult As New IngDTResult
        Dim tresult As New TransResult

        Dim spCreditLineStatus As String = "sp_update_credit_line"
        Dim spUCARStatus As String = "sp_update_recon"
        Dim spAddUCAR As String = ""

        Dim amountCredited As Decimal = 0
        Dim amountUCAR As Decimal = 0

        Dim creditStatus As String = ""
        Dim reconStatus As String = ""

        Dim newReconNo As String = ""

        Dim result As Boolean = False

        Try

            amountCredited = CDec(txtAmountCredited.Value)
            amountUCAR = CDec(txtUCARAmount.Value)

            'CLOSE CURRENT UC
            reconStatus = "RX"

            spUCARStatus = spUCARStatus & ";VAR|" & creditid &
                    ":VAR|" & reconno &
                    ":VAR|" & reconStatus &
                    ":VAR|AR"

            'UPDATE/ADD NEW UC/AR
            If amountCredited = amountUCAR Then

                creditStatus = "CX" 'closed credit line

                spCreditLineStatus = spCreditLineStatus & ";VAR|" & creditid & ":VAR|" & creditStatus

                dtresult = svc.IngDataTableMultiProc({spCreditLineStatus, spUCARStatus})

                If dtresult.isDataGet Then

                    tresult.isSuccessful = True
                    tresult.resultMsg = "Reconciliation successfully saved. (Status: BALANCED)"

                Else

                    tresult.resultMsg = "Error in saving BALANCED UC/AR: " & dtresult.DTErrorMsg
                    tresult.isSuccessful = False

                End If

            End If

            If amountCredited > amountUCAR Then

                spAddUCAR = "sp_ins_credit_ar"

                newReconNo = getUCARNo(creditid, "AR")

                spAddUCAR = spAddUCAR &
                    ";VAR|" & creditid &
                    ":VAR|" & newReconNo &
                    ":VAR|" & amountUCAR &
                    ":VAR|" & amountCredited &
                    ":VAR|" & amountCredited - amountUCAR &
                    ":VAR|" & userid


                dtresult = svc.IngDataTableMultiProc({spUCARStatus, spAddUCAR})

                If dtresult.isDataGet Then

                    tresult.resultMsg = "Reconciliation successfully saved. (Status: WITH AR)"


                Else

                    tresult.resultMsg = "Error in saving OVERREMITTED UC/AR: " & dtresult.DTErrorMsg
                    tresult.isSuccessful = False

                End If

            End If

            If amountCredited < amountUCAR Then

                spAddUCAR = "sp_ins_credit_uc"

                newReconNo = getUCARNo(creditid, "AR")

                spAddUCAR = spAddUCAR &
                    ";VAR|" & creditid &
                    ":VAR|" & newReconNo &
                    ":VAR|" & amountUCAR &
                    ":VAR|" & amountCredited &
                    ":VAR|" & amountUCAR - amountCredited &
                    ":VAR|" & userid


                dtresult = svc.IngDataTableMultiProc({spUCARStatus, spAddUCAR})

                If dtresult.isDataGet Then

                    tresult.resultMsg = "Reconciliation successfully saved. (Status: WITH AR)"


                Else

                    tresult.resultMsg = "Error in saving OVERREMITTED UC/AR: " & dtresult.DTErrorMsg
                    tresult.isSuccessful = False

                End If

            End If

        Catch ex As Exception

            tresult.isSuccessful = False
            tresult.resultMsg = ex.Message

        End Try


        Return tresult

    End Function

    Private Function getUCARNo(creditid As String, reconType As String) As String

        Dim reconNo As String = "001"

        Dim dtresult As New IngDTResult
        Dim svc As New Service1Client

        Dim strParams As String() = {
            "VAR|" & creditid,
            "VAR|" & reconType
            }

        dtresult = svc.IngDataTable("sp_get_max_reconno", strParams)

        If dtresult.isDataGet Then

            For Each dtRow As DataRow In dtresult.DataSetResult.Tables(0).Rows

                If Trim(dtRow(0).ToString) = "" Then

                    Exit For

                Else

                    reconNo = Val(dtRow(0).ToString) + 1
                    reconNo = reconNo.PadLeft(3, "0")


                End If

            Next

        End If

        Return reconNo

    End Function

End Class