Imports EcollReconFacility.EcollReconWCF

Public Class UncreditedTrans
    Inherits System.Web.UI.Page

    Dim userid As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If IsNothing(Session("ActiveUserID")) Then

            Response.Redirect("~/Login.aspx")

        Else

            userid = Session("ActiveUserID")

        End If

        loadControls()

    End Sub

    Protected Sub gvUncreditedTrans_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles gvUncreditedTrans.PageIndexChanging

        gvUncreditedTrans.PageIndex = e.NewPageIndex
        loadControls()

    End Sub

    Private Sub loadControls()


        dtpmodalCreditDate.Value = Today.ToString("MMMM dd, yyyy")

        gvUncreditedTrans.DataSource = getUncreditedTrans()
        gvUncreditedTrans.DataBind()

    End Sub

    Private Function getUncreditedTrans() As DataTable

        Dim dt As New DataTable

        With dt.Columns
            .Clear()
            .Add("rownumber")
            .Add("bankinsti")
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
                                CDate(dtRow(1).ToString).ToString("MMMM dd, yyyy"),
                                CDec(dtRow(2).ToString).ToString("#,###,##0.00")
                                })

                    rownumber = rownumber + 1

                Next

            End If

        Next


        Return dt
    End Function

End Class