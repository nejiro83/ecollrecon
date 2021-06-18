' NOTE: You can use the "Rename" command on the context menu to change the class name "Service1" in code, svc and config file together.
' NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.vb at the Solution Explorer and start debugging.

Imports EcollReconWCF
Imports Ingres.Client

Public Class EcollReconWS
    Implements IService1


    Dim dtResult As New IngDTResult
    Dim connStringEcoll As String = ConfigurationManager.ConnectionStrings("EcollConn").ConnectionString
    Dim connStringTKT As String = ConfigurationManager.ConnectionStrings("TKTDEMOHO").ConnectionString


    Public Sub New()


    End Sub

    Public Function GetData(ByVal value As Integer) As String Implements IService1.GetData
        Return String.Format("You entered: {0}", value)
    End Function

    Public Function GetDataUsingDataContract(ByVal composite As CompositeType) As CompositeType Implements IService1.GetDataUsingDataContract
        If composite Is Nothing Then
            Throw New ArgumentNullException("composite")
        End If
        If composite.BoolValue Then
            composite.StringValue &= "Suffix"
        End If
        Return composite
    End Function

    Public Function IngDataTable(ByVal ProcedureName As String, ByVal ParamArray IngFields() As String) As IngDTResult Implements IService1.IngDataTable

        Dim mydt As New IngDTResult
        mydt.isDataGet = False
        mydt.DataSetResult = Nothing
        mydt.DTErrorMsg = ""

        Dim comIngres As New IngresCommand
        Dim dr As New IngresDataAdapter

        Dim OutputTable As New DataTable
        Dim OutputSet As New DataSet

        With comIngres
            .Connection = IngConnection(False, connStringEcoll)
            .Parameters.Clear()
            .CommandText = ProcedureName
            .CommandType = Data.CommandType.StoredProcedure
        End With
        For i = 0 To UBound(IngFields)
            Dim StrDataType As String = IngFields(i).Split("|").GetValue(0)
            Dim StrValue As String = IngFields(i).Split("|").GetValue(1)
            Select Case StrDataType.ToUpper
                Case Is = "INT"
                    If Not IsNumeric(StrValue) Then
                        mydt.DTErrorMsg = "INVALID FOR DATA TYPE INT : '" & StrDataType & "'"
                        If comIngres.Connection.State = ConnectionState.Open Then
                            comIngres.Connection.Close()
                        End If
                        comIngres.Dispose()
                        Return mydt
                    End If
                    comIngres.Parameters.Add("Parameter" & i, IngresType.Int).Value = CInt(StrValue)
                Case Is = "VAR"
                    comIngres.Parameters.Add("Parameter" & i, IngresType.VarChar).Value = StrValue
                Case Is = "CHR"
                    comIngres.Parameters.Add("Parameter" & i, IngresType.Char).Value = StrValue
                Case Is = "DTE"
                    If Not IsDate(StrValue) Then
                        mydt.DTErrorMsg = "INVALID FOR DATA TYPE DATE : '" & StrDataType & "'"
                        If comIngres.Connection.State = ConnectionState.Open Then
                            comIngres.Connection.Close()
                        End If
                        comIngres.Dispose()
                        Return mydt
                    End If
                    comIngres.Parameters.Add("Parameter" & i, IngresType.Date).Value = CDate(StrValue)
                Case Is = "MON"
                    If Not IsNumeric(StrValue) Then
                        mydt.DTErrorMsg = "INVALID FOR DATA TYPE MONEY : '" & StrDataType & "'"
                        If comIngres.Connection.State = ConnectionState.Open Then
                            comIngres.Connection.Close()
                        End If
                        comIngres.Dispose()
                        Return mydt
                    End If
                    comIngres.Parameters.Add("Parameter" & i, IngresType.Decimal).Value = CDbl(StrValue)
                Case Else
                    mydt.DTErrorMsg = "INVALID DATA TYPE CODE : " & StrDataType
                    If comIngres.Connection.State = ConnectionState.Open Then
                        comIngres.Connection.Close()
                    End If
                    comIngres.Dispose()
                    Return mydt
            End Select
        Next
        Try
            dr.SelectCommand = comIngres
            dr.Fill(OutputTable)
            OutputSet.Tables.Add(OutputTable)
            mydt.DataSetResult = OutputSet
            mydt.isDataGet = True
        Catch x As IngresException
            mydt.DTErrorMsg = x.Message.ToString
        Catch x As Exception
            mydt.DTErrorMsg = x.Message.ToString
        Finally
            If comIngres.Connection.State = ConnectionState.Open Then
                comIngres.Connection.Close()
            End If
            comIngres.Dispose()
        End Try

        dtResult = mydt

        Return dtResult
    End Function

    Public Function IngDataTableCmdText(ByVal CommandText As String) As IngDTResult Implements IService1.IngDataTableCmdText

        Dim mydt As New IngDTResult
        mydt.isDataGet = False
        mydt.DataSetResult = Nothing
        mydt.DTErrorMsg = ""

        Dim comIngres As New IngresCommand
        Dim dr As New IngresDataAdapter

        Dim OutputSet As New DataSet

        With comIngres
            .Connection = IngConnection(True, connStringEcoll)
            Try
                .CommandText = CommandText
                If .ExecuteNonQuery() <> 0 Then
                    mydt.isDataGet = True
                    Dim myNewDS As New DataSet
                    Dim mydsdt As DataTable = myNewDS.Tables.Add("MYDT")
                    mydsdt.Rows.Add("1")
                    mydt.DataSetResult = myNewDS
                End If
            Catch x As IngresException
                mydt.DTErrorMsg = "IngDataTableCmdText " & x.Message.ToString
            Catch x As Exception
                mydt.DTErrorMsg = "IngDataTableCmdText " & x.Message.ToString
            Finally
                If comIngres.Connection.State = ConnectionState.Open Then
                    comIngres.Connection.Close()
                End If
                comIngres.Dispose()
            End Try
        End With

        Return mydt
    End Function

    Public Function IngDataTableMultiProc(ByVal ParamArray ProcedureName() As String) As IngDTResult Implements IService1.IngDataTableMultiProc

        Dim mydt As New IngDTResult
        mydt.isDataGet = False
        mydt.DataSetResult = Nothing
        mydt.DTErrorMsg = ""

        Dim comIngres As New IngresCommand
        Dim dr As New IngresDataAdapter

        Dim OutputSet As New DataSet

        With comIngres
            .Connection = IngConnection(False, connStringEcoll)
            Dim IngTransaction As IngresTransaction = .Connection.BeginTransaction()
            .Transaction = IngTransaction
            Try
                For Each StrProc As String In ProcedureName
                    Dim OutputTable As New DataTable
                    Dim procname As String = StrProc.Split(";").GetValue(0)
                    .Parameters.Clear()
                    .CommandText = procname
                    .CommandType = Data.CommandType.StoredProcedure
                    Dim paramnames As String = StrProc.Split(";").GetValue(1)
                    Dim i As Integer = 0
                    For Each params As String In paramnames.Split(":")
                        Dim StrDataType As String = params.Split("|").GetValue(0)
                        Dim StrValue As String = params.Split("|").GetValue(1)
                        Select Case StrDataType.ToUpper
                            Case Is = "INT"
                                If Not IsNumeric(StrValue) Then
                                    Throw New Exception("INVALID FOR DATA TYPE INT : '" & StrValue & "' FROM SP " & procname)
                                End If
                                comIngres.Parameters.Add("Parameter" & i.ToString(), IngresType.Int).Value = CInt(StrValue)
                            Case Is = "VAR"
                                comIngres.Parameters.Add("Parameter" & i.ToString(), IngresType.VarChar).Value = StrValue
                            Case Is = "CHR"
                                comIngres.Parameters.Add("Parameter" & i.ToString(), IngresType.Char).Value = StrValue
                            Case Is = "DTE"
                                If Not IsDate(StrValue) Then
                                    Throw New Exception("INVALID FOR DATA TYPE DATE : '" & StrValue & "' FROM SP " & procname)
                                End If
                                comIngres.Parameters.Add("Parameter" & i.ToString(), IngresType.Date).Value = CDate(StrValue)
                            Case Is = "MON"
                                If Not IsNumeric(StrValue) Then
                                    Throw New Exception(mydt.DTErrorMsg = "INVALID FOR DATA TYPE MONEY : '" & StrValue & "' FROM SP " & procname)
                                End If
                                comIngres.Parameters.Add("Parameter" & i.ToString(), IngresType.Decimal).Value = CDbl(StrValue)
                            Case Else
                                Throw New Exception(mydt.DTErrorMsg = "INVALID DATA TYPE CODE : " & StrDataType & "' FROM SP " & procname)
                        End Select
                        i += 1
                    Next
                    dr.SelectCommand = comIngres
                    dr.Fill(OutputTable)
                    OutputSet.Tables.Add(OutputTable)
                    mydt.DataSetResult = OutputSet
                Next
                IngTransaction.Commit()
                mydt.isDataGet = True
            Catch x As IngresException
                IngTransaction.Rollback()
                mydt.DTErrorMsg = "IngDataTableMultiProc " & x.Message.ToString
            Catch x As Exception
                IngTransaction.Rollback()
                mydt.DTErrorMsg = "IngDataTableMultiProc " & x.Message.ToString
            Finally
                If comIngres.Connection.State = ConnectionState.Open Then
                    comIngres.Connection.Close()
                End If
                comIngres.Dispose()
            End Try
        End With

        Return mydt
    End Function

    Private Shared Function IngConnection(ByVal isAdmin As Boolean, connString As String) As IngresConnection
        Dim ConIngres As New IngresConnection(connString)

        Try
            If ConIngres.State = ConnectionState.Closed Then
                ConIngres.Open()
                ExecuteReadLockSession(ConIngres)
                Return ConIngres
            End If
        Catch x As Exception

        End Try

        Return ConIngres
    End Function

    Private Shared Sub ExecuteReadLockSession(ByVal IngConn As IngresConnection)
        Dim IngresCm As New IngresCommand("set lockmode session where readlock=nolock", IngConn)
        If IngConn.State = ConnectionState.Open Then
            IngresCm.ExecuteNonQuery()
        End If
        IngresCm.Dispose()
    End Sub

#Region "Accounting Entries"

    Public Function genTransSRTNoNew(noOfSRT As Integer, tranMatrix As String) As IngDTResult Implements IService1.genTransSRTNoNew

        Dim myDT As New IngDTResult
        myDT.isDataGet = False
        myDT.DataSetResult = Nothing
        myDT.DTErrorMsg = ""

        Dim comIngres As New IngresCommand
        Dim dr As New IngresDataAdapter

        Dim OutputTable As New DataTable
        Dim OutputSet As New DataSet

        With comIngres
            .Connection = IngConnection(False, connStringEcoll)
            .CommandText = "sp_ecoll_gentransrtnonew"
            .CommandType = CommandType.StoredProcedure

            With .Parameters
                .Clear()
                .Add("noofsrt", IngresType.VarChar).Value = noOfSRT
                .Add("tran_matrix", IngresType.VarChar).Value = tranMatrix
            End With
        End With

        Try

            dr.SelectCommand = comIngres
            dr.Fill(OutputTable)
            OutputSet.Tables.Add(OutputTable)
            myDT.DataSetResult = OutputSet
            myDT.isDataGet = True

        Catch x As IngresException

            myDT.DTErrorMsg = x.Message.ToString

        Catch x As Exception

            myDT.DTErrorMsg = x.Message.ToString

        Finally

            If comIngres.Connection.State = ConnectionState.Open Then

                comIngres.Connection.Close()

            End If

            comIngres.Dispose()

        End Try

        dtResult = myDT

        Return dtResult
    End Function

    Public Function IngDataTableMultiProcWithTKT(ProcedureName() As String,
                                                 tktProcedureName As String, tktParams As String()) As IngDTResult Implements IService1.IngDataTableMultiProcWithTKT

        Dim mydt As New IngDTResult
        mydt.isDataGet = False
        mydt.DataSetResult = Nothing
        mydt.DTErrorMsg = ""

        Dim comIngres As New IngresCommand
        Dim dr As New IngresDataAdapter

        Dim OutputSet As New DataSet

        Dim IngTransaction As IngresTransaction = Nothing
        Dim IngTransactionTKT As IngresTransaction = Nothing

        Dim OutputTable As New DataTable

        With comIngres
            .Connection = IngConnection(False, connStringEcoll)
            IngTransaction = .Connection.BeginTransaction()
            .Transaction = IngTransaction
            Try
                For Each StrProc As String In ProcedureName
                    Dim procname As String = StrProc.Split(";").GetValue(0)
                    .Parameters.Clear()
                    .CommandText = procname
                    .CommandType = Data.CommandType.StoredProcedure
                    Dim paramnames As String = StrProc.Split(";").GetValue(1)
                    Dim i As Integer = 0
                    For Each params As String In paramnames.Split(":")
                        Dim StrDataType As String = params.Split("|").GetValue(0)
                        Dim StrValue As String = params.Split("|").GetValue(1)
                        Select Case StrDataType.ToUpper
                            Case Is = "INT"
                                If Not IsNumeric(StrValue) Then
                                    Throw New Exception("INVALID FOR DATA TYPE INT : '" & StrValue & "' FROM SP " & procname)
                                End If
                                comIngres.Parameters.Add("Parameter" & i.ToString(), IngresType.Int).Value = CInt(StrValue)
                            Case Is = "VAR"
                                comIngres.Parameters.Add("Parameter" & i.ToString(), IngresType.VarChar).Value = StrValue
                            Case Is = "CHR"
                                comIngres.Parameters.Add("Parameter" & i.ToString(), IngresType.Char).Value = StrValue
                            Case Is = "DTE"
                                If Not IsDate(StrValue) Then
                                    Throw New Exception("INVALID FOR DATA TYPE DATE : '" & StrValue & "' FROM SP " & procname)
                                End If
                                comIngres.Parameters.Add("Parameter" & i.ToString(), IngresType.Date).Value = CDate(StrValue)
                            Case Is = "MON"
                                If Not IsNumeric(StrValue) Then
                                    Throw New Exception(mydt.DTErrorMsg = "INVALID FOR DATA TYPE MONEY : '" & StrValue & "' FROM SP " & procname)
                                End If
                                comIngres.Parameters.Add("Parameter" & i.ToString(), IngresType.Decimal).Value = CDbl(StrValue)
                            Case Else
                                Throw New Exception(mydt.DTErrorMsg = "INVALID DATA TYPE CODE : " & StrDataType & "' FROM SP " & procname)
                        End Select
                        i += 1
                    Next
                    dr.SelectCommand = comIngres
                    dr.Fill(OutputTable)
                    OutputSet.Tables.Add(OutputTable)
                    mydt.DataSetResult = OutputSet
                Next

                If Trim(tktProcedureName) <> "" And (tktParams.Count > 0) Then

                    comIngres = New IngresCommand

                    With comIngres
                        .Connection = IngConnection(False, connStringTKT)
                        IngTransactionTKT = .Connection.BeginTransaction
                        .Parameters.Clear()
                        .CommandText = tktProcedureName
                        .CommandType = Data.CommandType.StoredProcedure
                    End With
                    For i = 0 To UBound(tktParams)
                        Dim StrDataType As String = tktParams(i).Split("|").GetValue(0)
                        Dim StrValue As String = tktParams(i).Split("|").GetValue(1)
                        Select Case StrDataType.ToUpper
                            Case Is = "INT"
                                If Not IsNumeric(StrValue) Then
                                    mydt.DTErrorMsg = "INVALID FOR DATA TYPE INT : '" & StrDataType & "'"
                                    If comIngres.Connection.State = ConnectionState.Open Then
                                        comIngres.Connection.Close()
                                    End If
                                    comIngres.Dispose()
                                    Return mydt
                                End If
                                comIngres.Parameters.Add("Parameter" & i, IngresType.Int).Value = CInt(StrValue)
                            Case Is = "VAR"
                                comIngres.Parameters.Add("Parameter" & i, IngresType.VarChar).Value = StrValue
                            Case Is = "CHR"
                                comIngres.Parameters.Add("Parameter" & i, IngresType.Char).Value = StrValue
                            Case Is = "DTE"
                                If Not IsDate(StrValue) Then
                                    mydt.DTErrorMsg = "INVALID FOR DATA TYPE DATE : '" & StrDataType & "'"
                                    If comIngres.Connection.State = ConnectionState.Open Then
                                        comIngres.Connection.Close()
                                    End If
                                    comIngres.Dispose()
                                    Return mydt
                                End If
                                comIngres.Parameters.Add("Parameter" & i, IngresType.Date).Value = CDate(StrValue)
                            Case Is = "MON"
                                If Not IsNumeric(StrValue) Then
                                    mydt.DTErrorMsg = "INVALID FOR DATA TYPE MONEY : '" & StrDataType & "'"
                                    If comIngres.Connection.State = ConnectionState.Open Then
                                        comIngres.Connection.Close()
                                    End If
                                    comIngres.Dispose()
                                    Return mydt
                                End If
                                comIngres.Parameters.Add("Parameter" & i, IngresType.Decimal).Value = CDbl(StrValue)
                            Case Else
                                mydt.DTErrorMsg = "INVALID DATA TYPE CODE : " & StrDataType
                                If comIngres.Connection.State = ConnectionState.Open Then
                                    comIngres.Connection.Close()
                                End If
                                comIngres.Dispose()
                                Return mydt
                        End Select
                    Next

                    dr.SelectCommand = comIngres
                    dr.Fill(OutputTable)
                    mydt.DataSetResult.Tables.Add(OutputTable)
                End If

                If IsNothing(IngTransactionTKT) = False Then IngTransactionTKT.Commit()
                IngTransaction.Commit()
                mydt.isDataGet = True

            Catch x As IngresException

                IngTransaction.Rollback()
                If IsNothing(IngTransactionTKT) = False Then IngTransactionTKT.Rollback()
                mydt.DTErrorMsg = "IngDataTableMultiProc " & x.Message.ToString

            Catch x As Exception

                IngTransaction.Rollback()
                If IsNothing(IngTransactionTKT) = False Then IngTransactionTKT.Rollback()
                mydt.DTErrorMsg = "IngDataTableMultiProc " & x.Message.ToString

            Finally

                If comIngres.Connection.State = ConnectionState.Open Then
                    comIngres.Connection.Close()
                End If
                comIngres.Dispose()

            End Try
        End With

        Return mydt

    End Function

#End Region
End Class
