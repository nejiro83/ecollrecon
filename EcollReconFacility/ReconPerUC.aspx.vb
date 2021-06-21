Imports EcollReconFacility.EcollReconWCF

Public Class ReconPerUC
    Inherits System.Web.UI.Page

    Dim userid As String = ""
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

            userid = Session("ActiveUserID")

        End If

        If IsPostBack Then

        Else

            loadControls()

        End If



    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click

        Dim tresult As TransResult = isSaved(Session("CreditID"), Session("ReconNo"))

        Session("ReconSaveStatus") = tresult.isSuccessful
        Session("ReconSaveMsg") = tresult.resultMsg

        Response.Redirect("~/UCMonitoring.aspx")

    End Sub


    Public Sub loadControls()

        Session("CreditID") = Request.QueryString("crid").ToString
        Session("ReconNo") = Request.QueryString("ucno").ToString

        getUCInfo(Session("CreditID"))

    End Sub

    Public Sub getUCInfo(creditid As String)

        Dim cmdText As String = "sp_sel_uc_info"

        Dim transDateFrom As String = ""
        Dim transDateTo As String = ""
        Dim dt As New DataTable

        Dim svc As New Service1Client
        Dim dtresult As New IngDTResult

        dtresult = svc.IngDataTable(cmdText, {"VAR|" & creditid})

        If dtresult.isDataGet Then

            For Each dtRow As DataRow In dtresult.DataSetResult.Tables(0).Rows

                bankinsticode = dtRow(0).ToString

                txtCollPartner.Text = dtRow(1).ToString
                txtReconDate.Text = CDate(dtRow(3).ToString).ToString("MMMM dd, yyyy")

                transDateFrom = CDate(dtRow(4).ToString).ToString("MMMM dd, yyyy")
                transDateTo = CDate(dtRow(5).ToString).ToString("MMMM dd, yyyy")

                txtTransDates.Text = transDateFrom & " - " & transDateTo

                txtUCAmount.Text = CDec(dtRow(6).ToString).ToString("#,###,##0.00")

                txtProcessType.Value = dtRow(7).ToString

            Next


        End If

        With dt.Columns
            .Add("transdate")
            .Add("amount")
            .Add("paytype")
            .Add("brcode")
            .Add("transcount")
        End With

        cmdText = "sp_sel_uc_trans"

        dtresult = svc.IngDataTable(cmdText,
                                    {"VAR|" & bankinsticode,
                                    "VAR|" & CDate(transDateFrom).ToString("MM/dd/yyyy"),
                                    "VAR|" & CDate(transDateTo).ToString("MM/dd/yyyy")})

        Dim totalLoadedAmount As Decimal = 0.0

        If dtresult.isDataGet Then

            For Each dtRow As DataRow In dtresult.DataSetResult.Tables(0).Rows

                dt.Rows.Add({
                            CDate(dtRow(0).ToString).ToString("MMMM dd, yyyy"),
                            CDec(dtRow(1).ToString).ToString("#,###,##0.00"),
                            dtRow(2).ToString,
                            dtRow(3).ToString,
                            CInt(dtRow(4).ToString).ToString("#,###,##0")
                })

                totalLoadedAmount = totalLoadedAmount + CDec(dtRow(1).ToString)

            Next

        End If

        txtLoadedAmount.Text = CDec(totalLoadedAmount).ToString("#,###,##0.00")
        txtBankInstiCode.Value = bankinsticode

        gvTransForUC.DataSource = dt
        gvTransForUC.DataBind()



    End Sub

    Public Function isSaved(creditid As String, reconno As String) As TransResult

        Dim svc As New Service1Client
        Dim dtresult As New IngDTResult
        Dim tresult As New TransResult

        Dim spCreditLineStatus As String = "sp_update_credit_line"
        Dim spUCARStatus As String = "sp_update_recon"
        Dim spAddUCAR As String = ""
        Dim spAddReconLines As String = "sp_ins_recon_lines"

        Dim UCAmount As Decimal = CDec(txtUCAmount.Text)
        Dim loadedAmount As Decimal = CDec(txtLoadedAmount.Text)

        Dim transDateFrom As String = CDate(txtTransDates.Text.Split("-")(0))
        Dim transDateTo As String = CDate(txtTransDates.Text.Split("-")(1))

        Dim creditStatus As String = ""
        Dim reconStatus As String = ""
        Dim reconType As String = ""

        Dim newReconNo As String = ""

        Dim spTKT As String() = {}

        Try

            'CLOSE CURRENT UC
            reconStatus = "RX"

            spUCARStatus = spUCARStatus & ";VAR|" & creditid &
                    ":VAR|" & reconno &
                    ":VAR|" & reconStatus &
                    ":VAR|UC"

            'UPDATE/ADD NEW UC/AR
            If loadedAmount = UCAmount Then

                creditStatus = "CX" 'closed credit line

                spCreditLineStatus = spCreditLineStatus & ";VAR|" & creditid & ":VAR|" & creditStatus

                spAddReconLines = spAddReconLines &
                    ";VAR|" & creditid &
                    ":VAR|" & transDateFrom &
                    ":VAR|" & transDateTo &
                    ":VAR|" & txtBankInstiCode.Value &
                    ":VAR|U"

                spTKT = generateSRT("BAL").ToArray

                dtresult = svc.IngDataTableMultiProcWithTKT(
                    {spCreditLineStatus, spUCARStatus, spAddReconLines}, spTKT)

                If dtresult.isDataGet Then

                    tresult.isSuccessful = True
                    tresult.resultMsg = "Reconciliation successfully saved. (Status: BALANCED)"

                Else

                    tresult.resultMsg = "Error in saving BALANCED UC/AR: " & dtresult.DTErrorMsg
                    tresult.isSuccessful = False

                End If

            End If

            If loadedAmount > UCAmount Then

                creditStatus = "CA" 'closed credit line

                spCreditLineStatus = spCreditLineStatus & ";VAR|" & creditid & ":VAR|" & creditStatus

                spAddUCAR = "sp_ins_credit_ar"

                newReconNo = getUCARNo(creditid, "AR")

                spAddUCAR = spAddUCAR &
                    ";VAR|" & creditid &
                    ":VAR|" & newReconNo &
                    ":VAR|" & UCAmount &
                    ":VAR|" & loadedAmount &
                    ":VAR|" & loadedAmount - UCAmount &
                    ":VAR|" & userid

                spAddReconLines = spAddReconLines &
                    ";VAR|" & creditid &
                    ":VAR|" & transDateFrom &
                    ":VAR|" & transDateTo &
                    ":VAR|" & txtBankInstiCode.Value &
                    ":VAR|A"

                spTKT = generateSRT("AR").ToArray

                dtresult = svc.IngDataTableMultiProcWithTKT(
                    {spCreditLineStatus, spUCARStatus, spAddReconLines}, spTKT)

                If dtresult.isDataGet Then

                    tresult.isSuccessful = True
                    tresult.resultMsg = "Reconciliation successfully saved. (Status: WITH AR)"


                Else

                    tresult.resultMsg = "Error in saving OVERREMITTED UC/AR: " & dtresult.DTErrorMsg
                    tresult.isSuccessful = False

                End If

            End If

            If loadedAmount < UCAmount Then

                creditStatus = "CU" 'closed credit line

                spCreditLineStatus = spCreditLineStatus & ";VAR|" & creditid & ":VAR|" & creditStatus

                spAddUCAR = "sp_ins_credit_uc"

                newReconNo = getUCARNo(creditid, "UC")

                spAddUCAR = spAddUCAR &
                    ";VAR|" & creditid &
                    ":VAR|" & newReconNo &
                    ":VAR|" & UCAmount &
                    ":VAR|" & loadedAmount &
                    ":VAR|" & UCAmount - loadedAmount &
                    ":VAR|" & userid

                spAddReconLines = spAddReconLines &
                    ";VAR|" & creditid &
                    ":VAR|" & transDateFrom &
                    ":VAR|" & transDateTo &
                    ":VAR|" & txtBankInstiCode.Value &
                    ":VAR|U"

                spTKT = generateSRT("UC").ToArray

                dtresult = svc.IngDataTableMultiProcWithTKT(
                    {spCreditLineStatus, spUCARStatus, spAddReconLines}, spTKT)

                If dtresult.isDataGet Then

                    tresult.isSuccessful = True
                    tresult.resultMsg = "Reconciliation successfully saved. (Status: WITH UC)"

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

    Private Function generateSRT(reconType As String) As List(Of String)

        Dim lstAcctng As New List(Of String)
        Dim outputAcctng As New AcctngEntries

        Dim dtresult As New IngDTResult
        Dim svc As New Service1Client

        Dim ingDTtrans As New IngDTResult

        Dim reconAmount As Decimal = 0.0

        dtresult = svc.IngDataTable("sp_get_reg_accts",
                                    {"VAR|" & txtProcessType.Value,
                                    "VAR|R", "VAR|" & txtBankInstiCode.Value})

        Select Case reconType
            Case "BAL"

                For Each dtRow As DataRow In dtresult.DataSetResult.Tables(0).Rows

                    If dtRow(2).ToString = "C" And dtRow(3).ToString = "BA" Then

                        outputAcctng.tranMatrixOther = outputAcctng.tranMatrixOther &
                            dtRow(0).ToString & CDec(txtUCAmount.Text).ToString.PadRight(13, " ") & "|"
                        outputAcctng.noOfOtherAccts += 1

                    End If

                Next

            Case "UC"

                reconAmount = CDec(txtUCAmount.Text) - CDec(txtLoadedAmount.Text)

                For Each dtRow As DataRow In dtresult.DataSetResult.Tables(0).Rows

                    If dtRow(2).ToString = "C" And dtRow(3).ToString = "UC" Then

                        outputAcctng.tranMatrixOther = outputAcctng.tranMatrixOther &
                            dtRow(0).ToString & reconAmount.ToString.PadRight(13, " ") & "|"
                        outputAcctng.noOfOtherAccts += 1

                    End If

                Next

            Case "AR"

                reconAmount = CDec(txtLoadedAmount.Text) - CDec(txtUCAmount.Text)

                For Each dtRow As DataRow In dtresult.DataSetResult.Tables(0).Rows

                    If dtRow(2).ToString = "C" And dtRow(3).ToString = "BA" Then

                        outputAcctng.tranMatrixOther = outputAcctng.tranMatrixOther &
                            dtRow(0).ToString & reconAmount.ToString.PadRight(13, " ") & "|"
                        outputAcctng.noOfOtherAccts += 1

                    End If

                Next

        End Select

        ingDTtrans = svc.genTransSRTNoNew(0, "")

        outputAcctng.tranNo = ingDTtrans.DataSetResult.Tables(0).Rows(0)(0).ToString
        outputAcctng.noOfSRT = 0
        outputAcctng.bankCode = txtBankInstiCode.Value
        outputAcctng.debitAmount = CDec(txtUCAmount.Text)
        outputAcctng.ecollUser = Session("ActiveUserID")
        outputAcctng.tranMatrixDueTo = ""

        lstAcctng.Add(outputAcctng.outputSP)

        Return lstAcctng
    End Function

End Class