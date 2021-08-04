Imports EcollReconFacility.EcollReconWCF

Public Class UncreditedTrans
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

        Else

            loadControls()

        End If



    End Sub

    Protected Sub btnModalSave_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim trResult As TransResult = SaveCreditLines()

        Dim message As String = ""

        If trResult.isSuccessful Then

            message = "Credit Lines successfully saved"

        Else

            message = trResult.errMsg

        End If

        Page.ClientScript.RegisterClientScriptBlock([GetType](),
                                                    "msgBox",
                                                    "MsgBox('" & message & "', 'Information');", True)



        Page.Response.Cache.SetCacheability(HttpCacheability.NoCache)



    End Sub

    Protected Sub gvUncreditedTrans_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles gvUncreditedTrans.PageIndexChanging

        gvUncreditedTrans.PageIndex = e.NewPageIndex
        loadControls()

    End Sub

    Private Sub loadControls()


        dtpmodalCreditDate.Text = Today.ToString("MMMM dd, yyyy")

        gvUncreditedTrans.DataSource = getUncreditedTrans()
        gvUncreditedTrans.DataBind()

    End Sub

    Private Function getUncreditedTrans() As DataTable

        Dim dt As New DataTable

        With dt.Columns
            .Clear()
            .Add("rownumber")
            .Add("bankinsticode")
            .Add("bankinsti")
            .Add("ecolltype")
            .Add("transdate")
            .Add("amount")
        End With

        Dim bankcodes As String() = Session("ActiveUserBankCodes").ToString.Split("|")
        Dim rownumber As Integer = 1

        For Each bankcode As String In bankcodes

            Dim dtresult As New IngDTResult
            Dim svc As New Service1Client

            dtresult = svc.IngDataTable("sp_get_uncredited_trans", {"VAR|" & bankcode})

            If dtresult.isDataGet Then

                For Each dtRow As DataRow In dtresult.DataSetResult.Tables(0).Rows

                    dt.Rows.Add({
                                rownumber,
                                dtRow(0).ToString,
                                dtRow(1).ToString,
                                dtRow(2).ToString,
                                CDate(dtRow(3).ToString).ToString("MMMM dd, yyyy"),
                                CDec(dtRow(4).ToString).ToString("#,###,##0.00")
                                })

                    rownumber = rownumber + 1

                Next

            End If

        Next


        Return dt
    End Function

    Private Function SaveCreditLines() As TransResult

        Dim isSaved As Boolean = False

        Dim trResult As New TransResult

        Try

            Dim bankinsticode As String = txtBankInstiCode.Value
            Dim ecolltype As String = txtEcollType.Value

            Dim modalCreditDate As String = CDate(dtpmodalCreditDate.Text).ToString("MM/dd/yyyy")
            Dim transDateFrom As String = CDate(txtTransDateHD.Value).ToString("MM/dd/yyyy")
            Dim transDateTo As String = CDate(txtTransDateHD.Value).ToString("MM/dd/yyyy")
            Dim amountCredited As String = CDec(Trim(txtAmountCredited.Text).Replace(",", "")).ToString

            Dim creditID As String = GetCreditID(bankinsticode, modalCreditDate)

            Dim strParams() As String = {
                "VAR|" & creditID,
                "VAR|" & transDateFrom,
                "VAR|" & transDateTo,
                "VAR|" & bankinsticode,
                "VAR|" & modalCreditDate,
                "VAR|" & amountCredited,
                "VAR|" & UCase(userid),
                "VAR|" & ecolltype
                }

            Dim svc As New Service1Client
            Dim dtresult As New IngDTResult

            dtresult = svc.IngDataTable("sp_ins_credit_lines", strParams)

            If dtresult.isDataGet Then

                gvUncreditedTrans.DataSource = getUncreditedTrans()
                gvUncreditedTrans.DataBind()

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

                    If bankinsticode.Substring(0, 1) = 0 Then
                        creditID = "0" & creditID
                    End If

                End If

            Next

        End If

        Return creditID

    End Function

End Class