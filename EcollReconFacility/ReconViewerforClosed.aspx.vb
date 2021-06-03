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


        Dim bankinsticode As String = Session("ActiveBankInstiCode")
        Dim bankinstiname As String = Session("ActiveBankInstiName")

        lblRMCollPartner.Text = bankinstiname
        lblRMSelectedTransDate.Text = Request.QueryString("t1") & " - " & Request.QueryString("t2")

        dt = getTransByCreditDate()

        gvRMTransPerDate.DataSource = dt
        gvRMTransPerDate.DataBind()

    End Sub

    Public Function getTransByCreditDate() As DataTable

        Dim creditDate As String = Request.QueryString("cd")
        Dim creditID As String = Request.QueryString("crid")
        Dim status As String = Request.QueryString("status")

        creditDate = CDate(creditDate).ToString("MM/dd/yyyy")

        Dim dt As New DataTable

        With dt.Columns
            .Clear()
            .Add("transdate")
            .Add("transrefno")
            .Add("paytype")
            .Add("amount")
        End With

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


        'WCF MODE

        Dim svc As New Service1Client
        Dim dtresult As New IngDTResult

        dtresult = svc.IngDataTable("sp_get_credit_line_trans", {"VAR|" & creditID})

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


        Return dt
    End Function
End Class