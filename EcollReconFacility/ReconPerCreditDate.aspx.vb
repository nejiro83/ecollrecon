Imports EcollReconFacility.EcollReconWCF
Imports EcollReconFacility.ADWS

Public Class WebForm3
    Inherits System.Web.UI.Page

    Dim userid As String = ""
    Dim creditLineStatus As String = ""

    Public Class TransResult
        Public TransType As Integer
        Public isSuccessful As Boolean = False
        Public errMsg As String = ""
    End Class

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        If IsNothing(Session("ActiveUserID")) Then

            Response.Redirect("~/Login.aspx")

        Else

            userid = Session("ActiveUserID")

        End If

        If IsPostBack Then

            If IsNothing(Session("IsCreditAmountUpdated")) = False And Session("IsCreditAmountUpdated") = True Then

                Page.ClientScript.RegisterClientScriptBlock(Me.GetType(),
                                                            "msgBox",
                                                            "MsgBox('" & Session("UpdateAmountMsg") & "', 'Successfully saved');", True)


                Session("IsCreditAmountUpdated") = Nothing
                Session("UpdateAmountMsg") = Nothing

            End If

        Else

            loadControls()

        End If

    End Sub

    Public Sub btnSave_Click(sender As Object, e As System.EventArgs) Handles btnSave.Click

        Dim trResult As TransResult = isStatusSaved(Request.QueryString("crid"))

        Dim msg As String = ""

        Select Case trResult.isSuccessful

            Case True

                msg = "Credit Line Status updated successfully"

            Case False

                msg = trResult.errMsg

        End Select

        Session("IsReconSaved") = trResult.isSuccessful
        Session("ReconMsg") = msg

        Response.Redirect("~/PendingRecon.aspx")

    End Sub

    Protected Sub btnAddTrans_Click(sender As Object, e As EventArgs) Handles btnAddTrans.Click

        Dim tr As TransResult = GetTransToExclude()

        If tr.isSuccessful = False Then

            lblMessage.Text = tr.errMsg

        Else

            lblMessage.Text = ""

        End If

        txtTransRefNo.Text = ""
        txtExcRemarks.Text = ""

        ClientScript.RegisterStartupScript(Me.GetType(), "kulaps", "$('#lst').collapse('show')", True)

    End Sub

    Protected Sub btnClearTrans_Click(sender As Object, e As EventArgs) Handles btnClearTrans.Click

        gvTrans.DataSource = New DataTable
        gvTrans.DataBind()

        If IsNothing(ViewState("gvTrans")) = False Then ViewState("gvTrans") = Nothing

        lblAmountExcluded.Text = 0.0

        ClientScript.RegisterStartupScript(Me.GetType(), "Popup", "$('#lst').collapse('show')", True)

    End Sub

    Protected Sub btnModalAdd_Click(sender As Object, e As EventArgs) Handles btnModalAdd.Click

        Dim result As TransResult = updateAmountCredited(Session("CreditID"),
                                                         txtSupervisorUserID.Text,
                                                         txtSupervisorPassword.Text,
                                                         txtUpdatedAmtCredited.Text,
                                                         txtAmCredited.Text, txtRemarks.Text)


        Dim msg As String = ""


        If result.isSuccessful Then

            msg = "Credited Amount updated successfully"

        Else

            msg = result.errMsg

        End If

        Session("IsCreditAmountUpdated") = result.isSuccessful
        Session("UpdateAmountMsg") = msg

        Response.Redirect(Request.RawUrl)


    End Sub

    Protected Sub gvTrans_RowDeleting(sender As Object, e As GridViewDeleteEventArgs)

        Dim index As Integer = Convert.ToInt32(e.RowIndex)
        Dim dt As DataTable = ViewState("gvTrans")

        Dim selectedAmt As String = dt.Rows(index)(3).ToString
        Dim selectedTrans As String = dt.Rows(index)(0).ToString

        If deleteExcludedTrans(selectedTrans) Then

            dt.Rows(index).Delete()

            ViewState("gvTrans") = dt

            gvTrans.DataSource = dt
            gvTrans.DataBind()

            If gvTrans.Rows.Count = 0 Then ViewState("gvTrans") = Nothing

            lblAmountExcluded.Text = CDec(lblAmountExcluded.Text - selectedAmt).ToString("#,###,##0.00")

        End If

    End Sub

    Public Sub loadControls()

        Session("CreditID") = Request.QueryString("crid").ToString

        lblAmountExcluded.Text = 0.0

        getCreditLineInfo(Session("CreditID"))

        Dim bankinsticode As String = Session("CreditID").ToString.Substring(0, 4)

        Dim dt As DataTable = getTransPerCreditLine(bankinsticode,
                                                    CDate(txtCreditDate.Text).ToString("MM/dd/yyyy"),
                                                    Session("CreditID").ToString,
                                                    creditLineStatus)

        txtTransDates.Text = dt.Rows(0)(0).ToString & " - " & dt.Rows(dt.Rows.Count - 1)(0).ToString

        With gvTransPerCreditLine
            .DataSource = dt
            .DataBind()
        End With

        Dim totalAmount As Decimal = 0.0
        Dim reconAmount As Decimal = 0.0

        For Each gvrow As GridViewRow In gvTransPerCreditLine.Rows

            totalAmount = totalAmount + CDec(gvrow.Cells(1).Text)

        Next

        reconAmount = totalAmount - CDec(txtAmCredited.Text)

        lblPanelTotalTransAmount.Text = CDec(totalAmount).ToString("#,###,##0.00")
        lblAmtToReconcile.Text = CDec(reconAmount).ToString("#,###,##0.00")


        If totalAmount > CDec(txtAmCredited.Text) Then

            lstExcluded.Visible = True

        Else

            lstExcluded.Visible = False

        End If


    End Sub

    Public Function getTransPerCreditLine(bankinsticode As String,
                                          creditDate As String,
                                          creditID As String,
                                          status As String) As DataTable

        Dim dt As New DataTable

        With dt.Columns
            .Add("transdate")
            .Add("amount")
            .Add("paytype")
            .Add("brcode")
            .Add("transcount")
        End With

        creditDate = CDate(creditDate).ToString("MM/dd/yyyy")

        Dim svc As New Service1Client
        Dim dtresult As New IngDTResult

        Dim strParam As String() = {
            "VAR|" & bankinsticode,
            "VAR|" & creditID,
            "VAR|" & creditDate,
            "VAR|" & status
            }

        dtresult = svc.IngDataTable("sp_get_credit_trans", strParam)

        If dtresult.isDataGet Then

            For Each dtRow As DataRow In dtresult.DataSetResult.Tables(0).Rows

                dt.Rows.Add({
                            CDate(dtRow(0).ToString).ToString("MMMM dd, yyyy"),
                            CDec(dtRow(1).ToString).ToString("#,###,##0.00"),
                            dtRow(2).ToString,
                            dtRow(3).ToString,
                            CInt(dtRow(4).ToString).ToString("#,###,##0")
                            })

            Next

        End If

        txtTransDates.Text = dt.Rows(0)(0).ToString & " - " & dt.Rows(dt.Rows.Count - 1)(0).ToString

        Return dt
    End Function

    Private Function isStatusSaved(creditid As String) As TransResult

        Dim trResult As New TransResult

        Try

            Dim svc As New Service1Client
            Dim dtresult As New IngDTResult

            Dim status As String = ""
            Dim cmdText As String = ""

            Dim cmdReconStatusSP As String = "sp_update_credit_line"
            Dim updateCreditParams As String = ""

            Dim cmdReconTypeSP As String = ""
            Dim reconTypeParams As String = ""
            Dim reconType As String = ""

            Dim amountcredited As Decimal = CDec(txtAmCredited.Text)
            Dim amountonfile As Decimal = CDec(lblPanelTotalTransAmount.Text)
            Dim varianceAmount As Decimal = 0.0
            Dim hasVariance As Boolean = False

            Dim strSPs As String() = {}

            If amountcredited > amountonfile Then

                hasVariance = True

                cmdReconTypeSP = "sp_ins_credit_uc"

                varianceAmount = amountcredited - amountonfile

                status = "CU"
                reconType = "UC"

            ElseIf amountcredited < amountonfile Then

                hasVariance = True

                cmdReconTypeSP = "sp_ins_credit_ar"

                varianceAmount = amountonfile - amountcredited

                status = "CA"
                reconType = "AR"

            ElseIf amountcredited = amountonfile Then

                status = "CX"

            Else

                status = "CX"

            End If

            cmdReconStatusSP = cmdReconStatusSP & ";VAR|" & creditid & ":VAR|" & status

            If hasVariance Then

                Dim reconNo As String = getUCARNo(creditid, reconType)

                reconTypeParams = "VAR|" & creditid &
                    ":VAR|" & amountcredited &
                    ":VAR|" & amountcredited &
                    ":VAR|" & amountonfile &
                    ":VAR|" & varianceAmount &
                    ":VAR|" & userid

                cmdReconTypeSP = cmdReconTypeSP & ";" & reconTypeParams

                strSPs = {cmdReconStatusSP, cmdReconTypeSP}

            Else

                strSPs = {cmdReconStatusSP}

            End If

            dtresult = svc.IngDataTableMultiProc(strSPs)

            If dtresult.isDataGet Then

                trResult.isSuccessful = True

            Else

                trResult.isSuccessful = False
                trResult.errMsg = "Failed to update Credit Lines. " & vbCrLf &
                    dtresult.DTErrorMsg

            End If

        Catch ex As Exception

            trResult.isSuccessful = False
            trResult.errMsg = "System Error: " & ex.Message

        End Try

        Return trResult

    End Function

    Private Function GetTransToExclude() As TransResult

        Dim trResult As New TransResult

        Dim creditID As String = Session("CreditID")

        Dim dt As New DataTable

        If IsNothing(ViewState("gvTrans")) = False Then

            dt = ViewState("gvTrans")

        Else

            With dt.Columns
                .Clear()
                .Add("transrefno")
                .Add("transdate")
                .Add("paytype")
                .Add("amount")
                .Add("remarks")
            End With

        End If

        Try

            Dim cmdText As String = "sp_ins_credit_excluded"

            Dim svc As New Service1Client
            Dim dtresult As New IngDTResult

            Dim strparam As String() = {
                    "VAR|" & creditID,
                    "VAR|" & txtTransRefNo.Text,
                    "VAR|" & txtExcRemarks.Text,
                    "VAR|" & userid
                }

            dtresult = svc.IngDataTable(cmdText, strparam)

            If dtresult.isDataGet And (dtresult.DataSetResult.Tables(0).Rows.Count > 0) Then

                For Each dtRow As DataRow In dtresult.DataSetResult.Tables(0).Rows

                    dt.Rows.Add({
                                dtRow(0).ToString,
                                CDate(dtRow(1).ToString).ToString("MMMM dd, yyyy"),
                                dtRow(2).ToString,
                                CDec(dtRow(3).ToString).ToString("#,###,##0.00"),
                                dtRow(4).ToString
                                })

                    lblAmountExcluded.Text = CDec(CDec(lblAmountExcluded.Text) +
                        CDec(dtRow(3).ToString)).ToString("#,###,##0.00")

                Next

            End If

            If dt.Rows.Count > 0 Then

                gvTrans.DataSource = dt
                gvTrans.DataBind()

                ViewState("gvTrans") = dt

                trResult.isSuccessful = True

            Else

                trResult.isSuccessful = False
                trResult.errMsg = "No record found"

            End If

        Catch ex As Exception

            trResult.isSuccessful = False
            trResult.errMsg = ex.Message

        End Try

        'Try

        '    Dim cmdText As String = "sp_get_credit_trans_byref"

        '    Dim svc As New Service1Client
        '    Dim dtresult As New IngDTResult

        '    Dim strparam As String() = {
        '        "VAR|" & txtTransRefNo.Text,
        '        "VAR|" & creditID
        '        }

        '    dtresult = svc.IngDataTable(cmdText, strparam)

        '    If dtresult.isDataGet Then

        '        For Each dtRow As DataRow In dtresult.DataSetResult.Tables(0).Rows

        '            dt.Rows.Add({
        '                        dtRow(0).ToString,
        '                        CDate(dtRow(1).ToString).ToString("MMMM dd, yyyy"),
        '                        dtRow(2).ToString,
        '                        CDec(dtRow(3).ToString).ToString("#,###,##0.00")
        '                        })

        '            lblAmountExcluded.Text = CDec(CDec(lblAmountExcluded.Text) +
        '                CDec(dtRow(3).ToString)).ToString("#,###,##0.00")

        '        Next

        '    End If

        '    If dt.Rows.Count > 0 Then

        '        gvTrans.DataSource = dt
        '        gvTrans.DataBind()

        '        ViewState("gvTrans") = dt

        '        trResult.isSuccessful = True

        '    Else

        '        trResult.isSuccessful = False
        '        trResult.errMsg = "No record found"

        '    End If

        'Catch ex As Exception

        '    trResult.isSuccessful = False
        '    trResult.errMsg = ex.Message

        'End Try

        Return trResult
    End Function

    Private Sub getCreditLineInfo(creditid As String)

        'CREDIT LINE MAIN INFO'
        Dim cmdText As String = "sp_get_creditline_info"

        Dim svc As New Service1Client
        Dim dtresult As New IngDTResult

        dtresult = svc.IngDataTable(cmdText, {"VAR|" & creditid})

        If dtresult.isDataGet Then

            For Each dtRow As DataRow In dtresult.DataSetResult.Tables(0).Rows

                txtCreditDate.Text = CDate(dtRow(0).ToString).ToString("MMMM dd, yyyy")

                txtCollPartner.Text = dtRow(5).ToString

                lblDepositoryBank.Text = dtRow(6).ToString
                lblContactPerson.Text = dtRow(7).ToString
                lblBankAccountNo.Text = dtRow(8).ToString
                lblContactNumber.Text = dtRow(9).ToString
                lblEmailAddress.Text = dtRow(10).ToString

                txtTransDates.Text = dtRow(1).ToString & " - " & dtRow(2).ToString
                txtAmCredited.Text = CDec(dtRow(3).ToString).ToString("#,###,##0.00")
                creditLineStatus = dtRow(4).ToString

            Next

        End If

        'LIST OF TRANSACTIONS TO EXCLUDE

        cmdText = "sp_sel_credit_excluded"

        dtresult = svc.IngDataTable(cmdText, {"VAR|" & creditid})

        If dtresult.isDataGet Then

            Dim dt As New DataTable

            With dt.Columns
                .Clear()
                .Add("transrefno")
                .Add("transdate")
                .Add("paytype")
                .Add("amount")
                .Add("remarks")
            End With

            For Each dtRow As DataRow In dtresult.DataSetResult.Tables(0).Rows


                dt.Rows.Add({dtRow(0).ToString,
                            CDate(dtRow(1).ToString).ToString("MMMM dd, yyyy"),
                            dtRow(2).ToString,
                            CDec(dtRow(3).ToString).ToString("#,###,##0.00"),
                            dtRow(4).ToString
                            })

                lblAmountExcluded.Text = CDec(IIf(Trim(lblAmountExcluded.Text) <> "", CDec(lblAmountExcluded.Text), 0.0) +
                        CDec(dtRow(3).ToString)).ToString("#,###,##0.00")

            Next

            gvTrans.DataSource = dt
            gvTrans.DataBind()

            ViewState("gvTrans") = dt

        End If

    End Sub

    Private Function updateAmountCredited(creditid As String,
                                          userID As String,
                                          password As String,
                                          updatedAmount As String,
                                          origAmount As String,
                                          remarks As String) As TransResult

        Dim result As New TransResult


        'THIS PART SHOULD BE REPLACED WITH ACTIVE DIRECTORY AUTHENTICATION
        Dim cmdText As String = "select userid from collection_users where lower(userid) = '" & userID & "' and usercode = '00'"

        Dim svc As New Service1Client
        Dim dtresult As New IngDTResult

        'Dim ws As New ADWS.Service

        'Dim loginResult As LoginResult = ws.AuthenticateUserWithDetails(userid, password)

        'If loginResult.isLoginValid Then

        dtresult = svc.IngDataTableCmdText(cmdText)

        If dtresult.isDataGet Then

            cmdText = "sp_ins_updated_credit_amount"

            svc = New Service1Client
            dtresult = New IngDTResult

            Dim params As String() = {
                "VAR|" & creditid,
                "VAR|" & CDec(origAmount),
                "VAR|" & CDec(updatedAmount),
                "VAR|" & remarks,
                "VAR|" & userID
                }

            dtresult = svc.IngDataTable(cmdText, params)

            If dtresult.isDataGet Then

                result.isSuccessful = True

            End If

        End If

        Return result

    End Function

    Private Function deleteExcludedTrans(transrefno As String) As Boolean

        Dim creditID As String = Session("CreditID")

        Dim svc As New Service1Client
        Dim dtresult As New IngDTResult

        Dim cmdText As String = "delete from collection_credit_excluded where transrefno = '" & transrefno & "' and creditid = '" & creditID & "'"

        dtresult = svc.IngDataTableCmdText(cmdText)

        Return dtresult.isDataGet

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


                End If

            Next

        End If

        Return reconNo

    End Function

End Class