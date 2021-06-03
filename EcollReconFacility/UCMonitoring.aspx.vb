Imports EcollReconFacility.EcollReconWCF

Public Class UCMonitoring
    Inherits System.Web.UI.Page

    Dim userid As String = ""

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

        End If


    End Sub
    Protected Sub gvUC_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles gvUC.PageIndexChanging

        gvUC.PageIndex = e.NewPageIndex
        loadControls()

    End Sub

    Private Sub gvUC_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles gvUC.RowDataBound

        If e.Row.RowType = DataControlRowType.DataRow Then


        End If

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


    Private Sub loadControls()

        Dim dtBankInsti As DataTable = getBankInsti()

        With ddlBankInsti
            .DataSource = dtBankInsti
            .DataTextField = dtBankInsti.Columns(1).ToString
            .DataValueField = dtBankInsti.Columns(7).ToString
            .DataBind()
        End With

        ddlBankInsti.SelectedIndex = 0

        loadUCAR()

    End Sub

    Private Function getUCAR(bankinsticode As String, reconType As String, status As String) As DataTable

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

        dtresult = svc.IngDataTable(cmdText, {"VAR|" & userid, "VAR|" & bankinsticode, "VAR|" & status})

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
                                      ddlReconStatus.SelectedValue)

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

End Class