Imports EcollReconFacility.EcollReconWCF
Imports EcollReconFacility.ADWS

Public Class WebForm3
    Inherits System.Web.UI.Page

    Dim userid As String = ""
    Dim creditLineStatus As String = ""
    Dim bankinsticode As String = ""

    Public Class TransResult
        Public TransType As Integer
        Public isSuccessful As Boolean = False
        Public resultMsg As String = ""

    End Class

    Private Class AcctngEntries

        Public tranNo As String = ""
        Public noOfSRT As Integer = 0
        Public bankCode As String = ""
        Public debitAmount As Decimal = 0.0
        Public ecollUser As String = ""
        Public particular As String = "sample"
        Public tktDate As Date = Today.ToString("MM/dd/yyyy")
        Public tranMatrixDueTo As String = ""
        Public noOfOtherAccts As Integer = 0
        Public tranMatrixOther As String = ""

        Public Function outputSP() As String

            outputSP = "sp_ecoll_ins_glentriesnew;" &
                "VAR|" & tranNo &
                ":INT|" & noOfSRT &
                ":VAR|" & bankCode &
                ":VAR|" & debitAmount &
                ":VAR|" & ecollUser &
                ":VAR|" & particular &
                ":VAR|" & tktDate &
                ":VAR|" & tranMatrixDueTo &
                ":INT|" & noOfOtherAccts &
                ":VAR|" & tranMatrixOther

            Return outputSP
        End Function

    End Class

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        If IsNothing(Session("ActiveUserID")) Then

            Response.Redirect("~/Login.aspx")

        Else

            If getCreditStatus(Request.QueryString("crid")) <> "CP" Then

                Response.Redirect("~/PendingRecon.aspx")

            End If

            userid = Session("ActiveUserID")

        End If

        If IsPostBack Then

        Else

            loadControls()

            If IsNothing(Session("IsCreditAmountUpdated")) = False And Session("IsCreditAmountUpdated") = True Then

                Page.ClientScript.RegisterClientScriptBlock(Me.GetType(),
                                                            "msgBox",
                                                            "MsgBox('" & Session("UpdateAmountMsg") & "', 'Successfully saved');", True)


                Session("IsCreditAmountUpdated") = Nothing
                Session("UpdateAmountMsg") = Nothing

            End If

        End If

    End Sub

    Public Sub btnSave_Click(sender As Object, e As System.EventArgs)

        Dim trResult As TransResult = isStatusSaved(Request.QueryString("crid"))

        Session("IsReconSaved") = trResult.isSuccessful
        Session("ReconMsg") = trResult.resultMsg

        Response.Redirect("~/PendingRecon.aspx")

    End Sub

    Protected Sub btnAddTrans_Click(sender As Object, e As EventArgs) Handles btnAddTrans.Click
        Dim tr As TransResult = GetTransToExclude()

        If tr.isSuccessful = False Then

            lblMessage.Text = tr.resultMsg

        Else

            lblMessage.Text = ""

        End If

        txtTransRefNo.Text = ""

    End Sub

    Protected Sub btnClearTrans_Click(sender As Object, e As EventArgs) Handles btnClearTrans.Click

        Dim svc As New Service1Client
        Dim dtresult As New IngDTResult

        Dim cmdText As String = "delete from collection_credit_excluded where creditid = '" & Request.QueryString("crid") & "'"

        dtresult = svc.IngDataTableCmdText(cmdText)

        If dtresult.isDataGet Then

            gvTrans.DataSource = New DataTable
            gvTrans.DataBind()

            'If IsNothing(ViewState("gvTrans")) = False Then ViewState("gvTrans") = Nothing

            lblAmountExcluded.Text = 0.0
            btnAddTrans.Enabled = True

        Else

            lblMessage.Text = dtresult.DTErrorMsg

        End If

    End Sub

    Protected Sub btnModalAdd_Click(sender As Object, e As EventArgs)

        Dim result As TransResult = updateAmountCredited(Session("CreditID"),
                                                         txtSupervisorUserID.Text,
                                                         txtSupervisorPassword.Text,
                                                         txtUpdatedAmtCredited.Text,
                                                         txtAmCredited.Text, txtRemarks.Text)



        Session("IsCreditAmountUpdated") = result.isSuccessful
        Session("UpdateAmountMsg") = result.resultMsg

        Response.Redirect(Request.RawUrl)


    End Sub

    Protected Sub gvTrans_RowDeleting(sender As Object, e As GridViewDeleteEventArgs)

        Dim index As Integer = Convert.ToInt32(e.RowIndex)
        'Dim dt As DataTable = ViewState("gvTrans")

        Dim dt As New DataTable

        With dt.Columns
            .Clear()
            .Add("transrefno")
            .Add("transdate")
            .Add("paytype")
            .Add("amount")
            .Add("remarks")
        End With

        Dim selectedAmt As String = gvTrans.Rows(index).Cells(3).Text
        Dim selectedTrans As String = gvTrans.Rows(index).Cells(0).Text

        For Each gRow As GridViewRow In gvTrans.Rows

            dt.Rows.Add({
                    gRow.Cells(0).Text,
                    gRow.Cells(1).Text,
                    gRow.Cells(2).Text,
                    gRow.Cells(3).Text,
                    gRow.Cells(4).Text})

        Next

        If deleteExcludedTrans(selectedTrans) Then

            dt.Rows(index).Delete()

            gvTrans.DataSource = dt
            gvTrans.DataBind()

            lblAmountExcluded.Text = CDec(lblAmountExcluded.Text - selectedAmt).ToString("#,###,##0.00")

            If lblAmountExcluded.Text = lblAmtToReconcile.Text Then

                btnAddTrans.Enabled = False

            Else

                btnAddTrans.Enabled = True

            End If

        Else

            lblAmountExcluded.Text = "error"

        End If

    End Sub

    Public Sub loadControls()

        Session("CreditID") = Request.QueryString("crid").ToString

        lblAmountExcluded.Text = 0.0

        getCreditLineInfo(Session("CreditID"))

        bankinsticode = Session("CreditID").ToString.Substring(0, 4)
        txtBankInstiCode.Value = bankinsticode

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

        If lblAmountExcluded.Text = lblAmtToReconcile.Text Then

            btnAddTrans.Enabled = False

        Else

            btnAddTrans.Enabled = True

        End If

        If totalAmount > CDec(txtAmCredited.Text) Then

            lstExcluded.Visible = True
            divExcluded.Visible = True

        Else

            lstExcluded.Visible = False
            divExcluded.Visible = False

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
            .Add("paymode")
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
                            CInt(dtRow(4).ToString).ToString("#,###,##0"),
                            dtRow(5).ToString
                            })

            Next

        End If

        Return dt
    End Function

    Private Function isStatusSaved(creditid As String) As TransResult

        Dim trResult As New TransResult

        Try

            Dim svc As New Service1Client
            Dim dtresult As New IngDTResult

            Dim status As String = ""
            Dim cmdText As String = ""
            Dim statusMsg As String = "Reconciliation Status: "

            Dim cmdReconStatusSP As String = "sp_update_credit_line"
            Dim updateCreditParams As String = ""

            Dim cmdReconTypeSP As String = ""
            Dim reconTypeParams As String = ""
            Dim reconType As String = ""

            Dim amountcredited As Decimal = CDec(txtAmCredited.Text)
            Dim amountonfile As Decimal = CDec(lblPanelTotalTransAmount.Text)
            Dim varianceAmount As Decimal = 0.0
            Dim hasVariance As Boolean = False

            Dim strSPs As New List(Of String)

            Dim spGLEntriesNew As String = "sp_ecoll_ins_glentriesnew"
            Dim tktParams As String() = {}

            If amountcredited > amountonfile Then

                hasVariance = True

                cmdReconTypeSP = "sp_ins_credit_uc"

                varianceAmount = amountcredited - amountonfile

                status = "CU"
                reconType = "UC"

                statusMsg = statusMsg & "UC"

            ElseIf amountcredited < amountonfile Then

                hasVariance = True

                cmdReconTypeSP = "sp_ins_credit_ar"

                varianceAmount = amountonfile - amountcredited

                status = "CA"
                reconType = "AR"

                statusMsg = statusMsg & "AR"

            ElseIf amountcredited = amountonfile Then

                status = "CX"
                statusMsg = statusMsg & "BALANCED/CLOSED"

            Else

                status = "CX"
                statusMsg = statusMsg & "NONE"

            End If

            cmdReconStatusSP = cmdReconStatusSP & ";VAR|" & creditid & ":VAR|" & status


            If hasVariance Then

                Dim reconNo As String = getUCARNo(creditid, reconType)

                reconTypeParams = "VAR|" & creditid &
                    ":VAR|" & reconNo &
                    ":VAR|" & amountcredited &
                    ":VAR|" & amountonfile &
                    ":VAR|" & varianceAmount &
                    ":VAR|" & userid

                cmdReconTypeSP = cmdReconTypeSP & ";" & reconTypeParams

                strSPs.Add(cmdReconStatusSP)
                strSPs.Add(cmdReconTypeSP)

            Else

                strSPs.Add(cmdReconStatusSP)

            End If


            Dim spTKT As List(Of String) = generateSRT(reconType, txtFloatDays.Value, txtProcessType.Value)

            For Each strProc As String In spTKT

                If strProc.Split("|")(0) = "SRT" Then

                    Dim spInsTicket As String = "sp_ins_recon_tickets;"
                    Dim srtNo As String = strProc.Split("|")(1)
                    Dim tktTranNo As String = srtNo.Split("-")(1)

                    spInsTicket = spInsTicket &
                        "VAR|" & creditid &
                        ":VAR|" & srtNo &
                        ":VAR|" & Session("ActiveUserID") &
                        ":VAR|" & tktTranNo

                    strSPs.Add(spInsTicket)

                    spTKT.Remove(strProc)

                    Exit For

                End If

            Next

            dtresult = svc.IngDataTableMultiProcWithTKT(strSPs.ToArray, spTKT.ToArray)

            If dtresult.isDataGet Then

                trResult.isSuccessful = True
                trResult.resultMsg = "Credit Line Status updated successfully (" & statusMsg & ")"

            Else

                trResult.isSuccessful = False
                trResult.resultMsg = "Failed to update Credit Lines. " & vbCrLf &
                    dtresult.DTErrorMsg

            End If

        Catch ex As Exception

            trResult.isSuccessful = False
            trResult.resultMsg = "System Error: " & ex.Message

        End Try

        Return trResult

    End Function

    Private Function GetTransToExclude() As TransResult

        Dim trResult As New TransResult

        Dim creditID As String = Session("CreditID")

        Dim dt As New DataTable

        With dt.Columns
            .Clear()
            .Add("transrefno")
            .Add("transdate")
            .Add("paytype")
            .Add("amount")
            .Add("remarks")
        End With

        Try

            Dim cmdText As String = "sp_ins_credit_excluded"

            Dim svc As New Service1Client
            Dim dtresult As New IngDTResult

            Dim strparam As String() = {
                    "VAR|" & creditID,
                    "VAR|" & txtTransRefNo.Text,
                    "VAR|" & ddlRemarks.SelectedValue,
                    "VAR|" & userid
                }

            dtresult = svc.IngDataTable(cmdText, strparam)


            If dtresult.isDataGet Then

                If (dtresult.DataSetResult.Tables(0).Rows.Count > 0) Then

                    Dim excAmount As Decimal = 0.0

                    For Each dtRow As DataRow In dtresult.DataSetResult.Tables(0).Rows

                        dt.Rows.Add({
                                    dtRow(0).ToString,
                                    CDate(dtRow(1).ToString).ToString("MMMM dd, yyyy"),
                                    dtRow(2).ToString,
                                    CDec(dtRow(3).ToString).ToString("#,###,##0.00"),
                                    dtRow(4).ToString
                                    })

                        'lblAmountExcluded.Text = CDec(CDec(lblAmountExcluded.Text) +
                        '    CDec(dtRow(3).ToString)).ToString("#,###,##0.00")

                        excAmount += CDec(dtRow(3).ToString)

                    Next


                    gvTrans.DataSource = dt
                    gvTrans.DataBind()

                    trResult.isSuccessful = True

                    lblAmountExcluded.Text = excAmount.ToString("#,###,##0.00")

                    If lblAmountExcluded.Text = lblAmtToReconcile.Text Then

                        btnAddTrans.Enabled = False

                    End If

                Else

                    trResult.isSuccessful = False
                    trResult.resultMsg = "No record found"

                End If

            Else

                trResult.isSuccessful = False
                trResult.resultMsg = dtresult.DTErrorMsg

            End If

            'If dt.Rows.Count > 0 Then

            '    gvTrans.DataSource = dt
            '    gvTrans.DataBind()

            '    trResult.isSuccessful = True

            'Else

            '    trResult.isSuccessful = False
            '    trResult.resultMsg = "No record found"

            'End If

        Catch ex As Exception

            trResult.isSuccessful = False
            trResult.resultMsg = ex.Message

        End Try

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

                txtProcessType.Value = dtRow(11).ToString
                txtFloatDays.Value = dtRow(12).ToString


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

            'ViewState("gvTrans") = dt

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

            If CDec(updatedAmount) > CDec(lblPanelTotalTransAmount.Text) Then

                cmdText = "delete from collection_credit_excluded where creditid = '" & creditid & "'"

                dtresult = svc.IngDataTableCmdText(cmdText)

                If dtresult.isDataGet Then

                    gvTrans.DataSource = New DataTable
                    gvTrans.DataBind()

                    lblAmountExcluded.Text = ""

                End If

            End If


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
                result.resultMsg = "Amount Credited updated successfully"

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
                    reconNo = reconNo.PadLeft(3, "0")


                End If

            Next

        End If


        Return reconNo

    End Function

    Private Function getCreditStatus(creditid) As String

        Dim dtresult As New IngDTResult
        Dim svc As New Service1Client

        Dim creditStatus As String = ""

        Dim cmdText As String = "sp_get_creditline_info"

        dtresult = svc.IngDataTable(cmdText, {"VAR|" & creditid})

        If dtresult.isDataGet And dtresult.DataSetResult.Tables(0).Rows.Count > 0 Then

            creditStatus = dtresult.DataSetResult.Tables(0).Rows(0)(4).ToString

        End If

        Return creditStatus

    End Function

    Private Function generateSRT(recontype As String, floatperiod As Integer, processtype As String) As List(Of String)

        Dim creditid As String = Request.QueryString("crid")

        Dim noofSRT As Integer = 0
        Dim tranMatrix1 As String = "" 'with Due To
        Dim totalAmount As Decimal = 0.0
        Dim noOfOtherAccts As Integer = 0
        Dim tranMatrix2 As String = ""


        Dim svc As New Service1Client
        Dim dtresult As New IngDTResult

        Dim outputAcctng As New AcctngEntries
        Dim lstAcctngEntries As New List(Of String)


        Dim ingDTAcctng As New IngDTResult

        ingDTAcctng = svc.IngDataTable("sp_get_reg_accts", {"VAR|" & txtBankInstiCode.Value})

        If ingDTAcctng.isDataGet = False Then

            Return lstAcctngEntries

        End If

        Dim reconAmount As Decimal = 0.0
        Dim ingDTTrans As New IngDTResult

        Select Case recontype
            Case "UC"

                reconAmount = CDec(txtAmCredited.Text - lblPanelTotalTransAmount.Text)

            Case "AR"

                reconAmount = CDec(lblPanelTotalTransAmount.Text - txtAmCredited.Text)

        End Select

        Dim dtPayTypeWithChecks As DataTable = getCreditTransPerPayType(creditid)

        'IF WITH CHECKS,  

        If dtPayTypeWithChecks.Rows.Count > 0 Then

            'entry 1 - DEBIT PCA, CREDIT BANK ACCOUNT

            For Each dtRow As DataRow In ingDTAcctng.DataSetResult.Tables(0).Rows

                If dtRow(2).ToString = "BA" Then

                    outputAcctng.tranMatrixOther = outputAcctng.tranMatrixOther &
                            dtRow(0).ToString & CDec(lblPanelTotalTransAmount.Text).ToString.PadRight(13, " ") & "|"

                    outputAcctng.debitAmount = CDec(lblPanelTotalTransAmount.Text)
                    outputAcctng.noOfOtherAccts += 1

                    ingDTTrans = svc.genTransSRTNoNew(0, "")

                    outputAcctng.tranNo = ingDTTrans.DataSetResult.Tables(0).Rows(0)(0).ToString
                    outputAcctng.noOfSRT = 0
                    outputAcctng.bankCode = txtBankInstiCode.Value

                    outputAcctng.ecollUser = Session("ActiveUserID")
                    outputAcctng.tktDate = Today.ToString("MM/dd/yyyy")
                    outputAcctng.tranMatrixDueTo = ""

                    lstAcctngEntries.Add(outputAcctng.outputSP)

                    Exit For

                End If

            Next

            'entry 2 - DEBIT PCA, DEBIT OTHERS
            outputAcctng.debitAmount = CDec(lblPanelTotalTransAmount.Text)

            For Each dtRow As DataRow In ingDTAcctng.DataSetResult.Tables(0).Rows

                Select Case dtRow(2).ToString

                    Case "MC"

                        Dim MCamount As Decimal = 0.0

                        For Each ptRow As DataRow In dtPayTypeWithChecks.Rows

                            If ptRow(0).ToString = "MC" Or
                                    ptRow(0).ToString = "MS" Or
                                    ptRow(0).ToString = "M2" Then

                                MCamount = MCamount + CDec(ptRow(1).ToString)

                            End If

                        Next

                        outputAcctng.tranMatrixOther = outputAcctng.tranMatrixOther &
                                dtRow(0).ToString & MCamount.ToString.PadRight(13, " ") & "|"

                        outputAcctng.noOfOtherAccts += 1

                    Case "ST"

                        Dim STamount As Decimal = 0.0

                        If dtRow(2).ToString = "C" Then

                            For Each ptRow As DataRow In dtPayTypeWithChecks.Rows

                                If ptRow(0).ToString = "ST" Or
                                    ptRow(0).ToString = "CL" Then

                                    STamount = STamount + CDec(ptRow(1).ToString)

                                End If

                            Next

                            outputAcctng.tranMatrixOther = outputAcctng.tranMatrixOther &
                                dtRow(0).ToString & STamount.ToString.PadRight(13, " ") & "|"

                            outputAcctng.noOfOtherAccts += 1

                        End If

                    Case "HL"


                End Select

            Next

            'add CREDIT DUE TO
            Dim dtDueto As DataTable = getCreditDueTo(creditid)

            For Each duetoRow As DataRow In dtDueto.Rows

                outputAcctng.tranMatrixDueTo = outputAcctng.tranMatrixDueTo &
                                    duetoRow(0).ToString.PadLeft(3, "0") & " " &
                                    duetoRow(2).ToString.PadRight(13, " ") & "|"

                outputAcctng.noOfSRT += 1

            Next

            ingDTTrans = svc.genTransSRTNoNew(outputAcctng.noOfSRT, outputAcctng.tranMatrixDueTo)

            outputAcctng.tranNo = ingDTTrans.DataSetResult.Tables(0).Rows(0)(0).ToString
            outputAcctng.bankCode = txtBankInstiCode.Value

            outputAcctng.ecollUser = Session("ActiveUserID")
            outputAcctng.particular = "sample"
            outputAcctng.tktDate = Today.ToString("MM/dd/yyyy")

            lstAcctngEntries.Add(outputAcctng.outputSP)

            Dim ecollSRT As String = Today.Year.ToString.Substring(Today.Year.ToString.Length - 2) &
                Now.DayOfYear.ToString.PadLeft(3, "0") &
                "-" & outputAcctng.tranNo


            lstAcctngEntries.Add("SRT|" & ecollSRT)

        End If

        'IF WITH FLOAT

        If floatperiod > 0 Then

            'DEBIT BANK ACCOUNT, CREDIT AR- COLLECTING BANKS
            For Each dtRow As DataRow In ingDTAcctng.DataSetResult.Tables(0).Rows

                If dtRow(2).ToString = "AB" Then

                    outputAcctng.tranMatrixOther = outputAcctng.tranMatrixOther &
                        dtRow(0).ToString & CDec(lblPanelTotalTransAmount.Text).ToString.PadRight(13, " ") & "|"

                    outputAcctng.debitAmount = CDec(lblPanelTotalTransAmount.Text)
                    outputAcctng.noOfOtherAccts += 1

                    ingDTTrans = svc.genTransSRTNoNew(0, "")

                    outputAcctng.tranNo = ingDTTrans.DataSetResult.Tables(0).Rows(0)(0).ToString
                    outputAcctng.noOfSRT = 0
                    outputAcctng.bankCode = txtBankInstiCode.Value

                    outputAcctng.ecollUser = Session("ActiveUserID")
                    outputAcctng.tktDate = Today.ToString("MM/dd/yyyy")
                    outputAcctng.tranMatrixDueTo = ""

                    lstAcctngEntries.Add(outputAcctng.outputSP)

                    Exit For

                End If

            Next

        End If

        'ACCOUNTING ENTRY FOR UC / AR
        If recontype = "UC" Or recontype = "AR" Then

            For Each dtRow As DataRow In ingDTAcctng.DataSetResult.Tables(0).Rows

                Select Case recontype
                    Case "UC"

                        If dtRow(2).ToString = "UC" Then

                            outputAcctng.tranMatrixOther = outputAcctng.tranMatrixOther &
                                        dtRow(0).ToString & reconAmount.ToString.PadRight(13, " ") & "|"
                            outputAcctng.noOfOtherAccts += 1

                        End If

                    Case "AR"

                        If dtRow(2).ToString = "BA" Then

                            outputAcctng.tranMatrixOther = outputAcctng.tranMatrixOther &
                                        dtRow(0).ToString & reconAmount.ToString.PadRight(13, " ") & "|"
                            outputAcctng.noOfOtherAccts += 1

                        End If

                End Select

            Next

            ingDTTrans = svc.genTransSRTNoNew(0, "")

            outputAcctng.tranNo = ingDTTrans.DataSetResult.Tables(0).Rows(0)(0).ToString
            outputAcctng.noOfSRT = 0
            outputAcctng.bankCode = txtBankInstiCode.Value
            outputAcctng.debitAmount = reconAmount
            outputAcctng.ecollUser = Session("ActiveUserID")
            outputAcctng.tranMatrixDueTo = ""

            lstAcctngEntries.Add(outputAcctng.outputSP)

        End If

        Return lstAcctngEntries

    End Function

    Private Function getCreditTransPerPayType(creditid As String) As DataTable
        Dim dt As New DataTable

        With dt.Columns
            .Clear()
            .Add("paytype")
            .Add("amount")
        End With

        Dim dtresult As New IngDTResult
        Dim svc As New Service1Client

        dtresult = svc.IngDataTable("sp_get_credit_transperpaytype", {"VAR|" & creditid})

        If dtresult.isDataGet Then

            For Each dtRow As DataRow In dtresult.DataSetResult.Tables(0).Rows

                dt.Rows.Add({dtRow(0).ToString, dtRow(1).ToString})

            Next

        End If

        Return dt
    End Function

    Private Function getCreditDueTo(creditid As String) As DataTable
        Dim dt As New DataTable

        With dt.Columns
            .Clear()
            .Add("duetobranch")
            .Add("paytype")
            .Add("amount")
        End With

        Dim dtresult As New IngDTResult
        Dim svc As New Service1Client

        dtresult = svc.IngDataTable("sp_get_credit_dueto", {"VAR|" & creditid})

        If dtresult.isDataGet Then

            For Each dtRow As DataRow In dtresult.DataSetResult.Tables(0).Rows

                dt.Rows.Add({dtRow(0).ToString, dtRow(1).ToString, dtRow(2).ToString})

            Next

        End If

        Return dt


    End Function

End Class