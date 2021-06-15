Imports EcollReconFacility.EcollReconWCF
Imports Ingres.Client
Imports System.Configuration

Public Class WebForm8
    Inherits System.Web.UI.Page

    Dim userid As String = ""
    Dim dt As New DataTable

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

    Protected Sub gvRMTransPerDate_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles gvRMTransPerDate.PageIndexChanging

        gvRMTransPerDate.PageIndex = e.NewPageIndex
        loadControls()

    End Sub

    Public Sub loadControls()

        Dim creditid As String = Request.QueryString("crid")

        getCreditLineInfo(creditid)

        dt = getTransByCreditDate(creditid)

        gvRMTransPerDate.DataSource = dt
        gvRMTransPerDate.DataBind()

        If IsNothing(Session("ClosedReconBackPage")) = False Then

            btnBack.PostBackUrl = Session("ClosedReconBackPage")

        Else

            btnBack.PostBackUrl = "#"

        End If

    End Sub

    Public Function getTransByCreditDate(creditid As String) As DataTable

        Dim dt As New DataTable

        With dt.Columns
            .Clear()
            .Add("transdate")
            .Add("transrefno")
            .Add("paytype")
            .Add("amount")
        End With


        'WCF MODE

        Dim svc As New Service1Client
        Dim dtresult As New IngDTResult

        dtresult = svc.IngDataTable("sp_get_credit_line_trans", {"VAR|" & creditid})

        If dtresult.isDataGet Then

            For Each dtRow As DataRow In dtresult.DataSetResult.Tables(0).Rows

                dt.Rows.Add({
                            CDate(dtRow(0).ToString).ToString("MMMM dd, yyyy"),
                            dtRow(1).ToString,
                            dtRow(2).ToString,
                            CDec(dtRow(3).ToString).ToString("#,###,##0.00")}
                            )


            Next

        End If

        'INGRES CLIENT MODE

        'Dim connstring As String = ConfigurationManager.ConnectionStrings("EcollConn").ToString

        'Dim ingConn As New IngresConnection(connstring)

        'ingConn.Open()

        'Dim cmd As New IngresCommand("set lockmode session where readlock = nolock", ingConn)

        'cmd.ExecuteNonQuery()

        'cmd = New IngresCommand()

        'With cmd
        '    .Connection = ingConn
        '    .CommandType = CommandType.StoredProcedure
        '    .CommandText = "sp_get_credit_line_trans"

        '    With .Parameters
        '        .Clear()
        '        .Add("creditid", IngresType.VarChar).Value = creditID
        '    End With
        'End With

        'Dim dr As IngresDataReader = cmd.ExecuteReader

        'If dr.HasRows Then

        '    While dr.Read

        '        dt.Rows.Add({
        '    CDate(dr(0).ToString).ToString("MMMM dd, yyyy"),
        '    dr(1).ToString,
        '    dr(2).ToString,
        '    CDec(dr(3).ToString).ToString("#,###,##0.00")})

        '    End While

        'End If

        'dr.Close()

        'ingConn.Close()


        Return dt
    End Function

    Private Sub getCreditLineInfo(creditid As String)

        'CREDIT LINE MAIN INFO'
        Dim cmdText As String = "sp_get_creditline_info"

        Dim svc As New Service1Client
        Dim dtresult As New IngDTResult

        dtresult = svc.IngDataTable(cmdText, {"VAR|" & creditid})

        If dtresult.isDataGet Then

            For Each dtRow As DataRow In dtresult.DataSetResult.Tables(0).Rows

                lblRMCollPartner.Text = dtRow(5).ToString
                lblRMSelectedTransDate.Text = CDate(dtRow(1).ToString).ToString("MMMM dd, yyyy") & " - " &
                    CDate(dtRow(2).ToString).ToString("MMMM dd, yyyy")

            Next

        End If

    End Sub
End Class