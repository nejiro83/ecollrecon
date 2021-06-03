
Imports CryptSecure.Process

Imports EcollReconFacility.ADWS
Imports EcollReconFacility.EcollReconWCF
Public Class WebForm1
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If IsPostBack Then

            If IsNothing(Session("ErrorMsg")) = False Then

                lblMessage.Text = Session("ErrorMsg")

            End If

        End If

    End Sub

    Protected Sub btnLogin_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnLogin.Click

        If isLoginValid(txtUserName.Text, txtPassword.Text) Then

            Dim encryptedUserID As String = Encrypt(Session("ActiveUserID"), "!#$a54?3")

            'Response.Redirect("TransToRecon.aspx?id=" & encryptedUserID)

            Response.Redirect("PendingRecon.aspx")

        Else

            Session("ErrorMsg") = "Invalid Username/Password/User Role"

            lblMessage.Text = Session("ErrorMsg")

        End If

    End Sub

    Private Function isLoginValid(ByVal userid As String, ByVal password As String) As Boolean

        Dim isValid As Boolean = False

        'Dim ws As New ADWS.Service

        'Dim loginResult As LoginResult = ws.AuthenticateUserWithDetails(userid, password)

        'If loginResult.isLoginValid Then

        Dim dtResult As New IngDTResult
        Dim svc As New Service1Client

        dtResult = svc.IngDataTable("get_userinfo", {"VAR|" & UCase(userid)})

        If dtResult.isDataGet Then

            Dim dt As DataTable = dtResult.DataSetResult.Tables(0)

            If dt.Rows.Count > 0 Then

                Session("ActiveUserFullName") = dt.Rows(0)(0).ToString
                Session("ActiveUserBankCodes") = dt.Rows(0)(2).ToString

                isValid = True

            Else

                isValid = False

            End If

            Session("ActiveUserID") = userid

        Else

            isValid = False

        End If

        'Else

        '    isValid = False

        'End If

        Return isValid
    End Function

End Class