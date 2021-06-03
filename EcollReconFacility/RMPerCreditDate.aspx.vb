

Public Class WebForm6
    Inherits System.Web.UI.Page

    Dim connString As String = ConfigurationManager.ConnectionStrings("EcollConn").ConnectionString

    Public Class TransResult
        Public TransType As Integer
        Public isSuccessful As Boolean = False
        Public errMsg As String = ""
    End Class

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not IsPostBack Then

            loadControls()

        End If

    End Sub

    Protected Sub btnAddTrans_Click(sender As Object, e As EventArgs) Handles btnAddTrans.Click

        ClientScript.RegisterStartupScript(Me.GetType(), "Popup", "$('#creditTransModal').modal('show'); $('#lst').collapse('show'); ", True)
        'ClientScript.RegisterStartupScript(Me.GetType(), "kulaps", "$('#lst').collapse('show')", True)

    End Sub

    Protected Sub btnModalAdd_Click(sender As Object, e As EventArgs) Handles btnModalAdd.Click

        Dim tr As TransResult = GetTransToExclude()

        If tr.isSuccessful = False Then

            lblMessage.Text = tr.errMsg

        Else

            lblMessage.Text = ""

        End If

        txtTransRefno.Text = ""
        txtPayorLName.Text = ""
        txtPayorFName.Text = ""
        txtPayorMName.Text = ""

        ClientScript.RegisterStartupScript(Me.GetType(), "kulaps", "$('#lst').collapse('show')", True)

    End Sub

    Protected Sub gvTrans_RowDeleting(sender As Object, e As GridViewDeleteEventArgs)

        Dim index As Integer = Convert.ToInt32(e.RowIndex)
        Dim dt As DataTable = ViewState("gvTrans")

        Dim selectedAmt As String = dt.Rows(index)(6).ToString

        dt.Rows(index).Delete()

        ViewState("gvTrans") = dt

        gvTrans.DataSource = dt
        gvTrans.DataBind()

        If gvTrans.Rows.Count = 0 Then ViewState("gvTrans") = Nothing

        lblAmountExcluded.Text = CDec(lblAmountExcluded.Text - selectedAmt).ToString("#,###,##0.00")

    End Sub

    Protected Sub btnClearTrans_Click(sender As Object, e As EventArgs) Handles btnClearTrans.Click

        gvTrans.DataSource = New DataTable
        gvTrans.DataBind()

        If IsNothing(ViewState("gvTrans")) = False Then ViewState("gvTrans") = Nothing

        lblAmountExcluded.Text = 0.0

        ClientScript.RegisterStartupScript(Me.GetType(), "Popup", "$('#lst').collapse('show')", True)

    End Sub

    Public Sub loadControls()

        Dim bankinsticode As String = Session("ActiveBankInstiCode")
        Dim bankinstiname As String = Session("ActiveBankInstiName")

        lblRMCollPartner.Text = bankinstiname
        lblRMSelectedTransDate.Text = Request.QueryString("t1") & " - " & Request.QueryString("t2")
        lblTotalAmountChecked.Text = Request.QueryString("ar")

        lblAmountExcluded.Text = 0.0

        gvTrans.DataSource = New DataTable
        gvTrans.DataBind()

    End Sub

    Public Function getTransByCreditDate() As DataTable

        Dim dt As New DataTable

        'Dim creditDate As String = Request.QueryString("cd")
        'Dim creditID As String = Request.QueryString("crid")

        'creditDate = CDate(creditDate).ToString("MM/dd/yyyy")

        'With dt.Columns
        '    .Clear()
        '    .Add("transdate")
        '    .Add("transrefno")
        '    .Add("paytype")
        '    .Add("pagibigid")
        '    .Add("lname")
        '    .Add("fname")
        '    .Add("mname")
        '    .Add("amount")
        'End With

        'Dim ingConn As New IngresConnection(connString)

        'ingConn.Open()

        'Dim cmdText As String = "select m.transdate, c.transrefno, m.paytype, o.pagibigid, o.lname, o.fname, o.midname, m.amount " &
        '    " from collection_master m inner join collection_otc o" &
        '    " on m.transrefno = o.transrefno " &
        '    "inner join collection_credit_lines c " &
        '    " on m.transrefno = c.transrefno " &
        '    " inner join collection_credit_mas d " &
        '    " on c.creditid = d.creditid " &
        '    " where d.creditdate = '" & creditDate & "'" &
        '    " and d.creditid = '" & creditID & "'"

        'Dim cmd As New IngresCommand(cmdText, ingConn)

        'Dim dr As IngresDataReader = cmd.ExecuteReader

        'If dr.HasRows Then

        '    While dr.Read

        '        dt.Rows.Add({
        '                    CDate(dr(0).ToString).ToShortDateString,
        '                    dr(1).ToString,
        '                    dr(2).ToString,
        '                    dr(3).ToString,
        '                    dr(4).ToString,
        '                    dr(5).ToString,
        '                    dr(6).ToString,
        '                    dr(7).ToString})

        '    End While


        'End If

        'dr.Close()
        'ingConn.Close()



        Return dt
    End Function

    Private Function GetTransToExclude() As TransResult

        Dim trResult As New TransResult

        'Dim ingConn As New IngresConnection(connString)

        'Dim creditID As String = Request.QueryString("crid")

        'Dim dt As New DataTable

        'With dt.Columns
        '    .Clear()
        '    .Add("transrefno")
        '    .Add("paytype")
        '    .Add("pagibigid")
        '    .Add("lname")
        '    .Add("fname")
        '    .Add("mname")
        '    .Add("amount")
        'End With

        'ingConn.Open()

        'Try

        '    SetLockModeSession(ingConn)

        '    Dim cmdText As String = "select c.transrefno, m.paytype, o.pagibigid, o.lname, o.fname, o.midname, m.amount " &
        '    " from collection_master m inner join collection_otc o" &
        '    " On m.transrefno = o.transrefno " &
        '    "inner join collection_credit_lines c " &
        '    " On m.transrefno = c.transrefno where (c.creditid = '" & creditID & "'" &
        '    " and c.transdate = '" & dtpTransDate.Value & "')" &
        '    " and (c.transrefno = '" & txtTransRefno.Text & "' " &
        '    " or (o.lname = '" & UCase(txtPayorLName.Text) & "' and o.fname = '" & UCase(txtPayorFName.Text) & "'))"

        '    Dim cmd As New IngresCommand(cmdText, ingConn)

        '    Dim dr As IngresDataReader = cmd.ExecuteReader

        '    If dr.HasRows Then

        '        While dr.Read

        '            dt.Rows.Add({
        '         dr(0).ToString,
        '         dr(1).ToString,
        '         dr(2).ToString,
        '         dr(3).ToString,
        '         dr(4).ToString,
        '         dr(5).ToString,
        '         dr(6).ToString})

        '            lblAmountExcluded.Text = CDec(CDec(lblAmountExcluded.Text) + CDec(dr(6).ToString)).ToString("#,###,##0.00")

        '        End While

        '    End If

        '    dr.Close()

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

        'ingConn.Close()

        Return trResult
    End Function

End Class