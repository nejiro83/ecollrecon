Imports EcollReconFacility.EcollReconWCF

Public Class ReverseTrans
    Inherits System.Web.UI.Page

    Dim userid As String = ""
    Dim connString As String = ConfigurationManager.ConnectionStrings("EcollConn").ConnectionString

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If IsNothing(Session("ActiveUserID")) Then

            Response.Redirect("~/Login.aspx")

        Else

            userid = Session("ActiveUserID")

        End If

        If IsPostBack Then

        Else

            ViewState("gvTrans") = New DataTable

            gvTrans.DataSource = ViewState("gvTrans")
            gvTrans.DataBind()

        End If

    End Sub
    Protected Sub btnSearchTrans_Click(sender As Object, e As EventArgs) Handles btnSearchTrans.Click

        Dim dt As DataTable = getTransInfo(txtTransrefno.Text)

        txtTransrefno.Text = ""

        Dim dtViewState As DataTable = IIf(IsNothing(ViewState("gvTrans")), New DataTable, ViewState("gvTrans"))

        If IsNothing(ViewState("gvTrans")) Or
            (dtViewState.Rows.Count = 0 Or dtViewState.Columns.Count = 0) Then

            dtViewState.Merge(dt, True)
        Else

            dtViewState = ViewState("gvTrans")

            If dt.Rows.Count = 0 Then

                lblMessage.Text = "Transaction not found"

            Else

                lblMessage.Text = ""

                For Each dRow As DataRow In dt.Rows

                    If dtViewState.Rows.Contains(dRow(0).ToString) = False Then

                        dtViewState.Rows.Add(dRow.ItemArray)

                    Else

                        lblMessage.Text = "Selected Transaction is already in the list"

                    End If

                Next

            End If

        End If

        ViewState("gvTrans") = dtViewState

        gvTrans.DataSource = dtViewState
        gvTrans.DataBind()

    End Sub

    Protected Sub gvTrans_RowDeleting(sender As Object, e As GridViewDeleteEventArgs)

        Dim index As Integer = Convert.ToInt32(e.RowIndex)
        Dim dt As DataTable = ViewState("gvTrans")

        Dim selectedAmt As String = dt.Rows(index)(3).ToString

        dt.Rows(index).Delete()

        ViewState("gvTrans") = dt

        gvTrans.DataSource = dt
        gvTrans.DataBind()

        If gvTrans.Rows.Count = 0 Then ViewState("gvTrans") = Nothing

    End Sub
    Protected Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click

        gvTrans.DataSource = New DataTable
        gvTrans.DataBind()

        ViewState("gvTrans") = Nothing

    End Sub

    Private Function getTransInfo(transrefno As String) As DataTable

        Dim dt As New DataTable

        With dt.Columns
            .Clear()
            .Add("transrefno")
            .Add("bankinsti")
            .Add("transdate")
            .Add("amount")
            .Add("ticketno")
        End With

        Dim dtColl() As DataColumn = Nothing
        dtColl = {dt.Columns(0)}
        dt.PrimaryKey = dtColl


        Dim cmdText As String = "sp_get_trans_reversal"
        Dim rownumber As Integer = 1

        Dim svc As New Service1Client
        Dim dtresult As New IngDTResult

        dtresult = svc.IngDataTable(cmdText, {
            "VAR|" & transrefno, "VAR|" & userid
                                    })

        If dtresult.isDataGet Then

            For Each dtRow As DataRow In dtresult.DataSetResult.Tables(0).Rows

                dt.Rows.Add({
                    dtRow(0).ToString,
                    dtRow(1).ToString,
                    CDate(dtRow(2).ToString).ToString("MMMM dd, yyyy"),
                  CDec(dtRow(3).ToString).ToString("#,###,##0.00"),
                    dtRow(4).ToString
                            })

            Next

        End If

        Return dt

    End Function


End Class