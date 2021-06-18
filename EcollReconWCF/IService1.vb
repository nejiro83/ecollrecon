﻿
' NOTE: You can use the "Rename" command on the context menu to change the interface name "IService1" in both code and config file together.
<ServiceContract()>
Public Interface IService1

    <OperationContract()>
    Function GetData(ByVal value As Integer) As String

    <OperationContract()>
    Function GetDataUsingDataContract(ByVal composite As CompositeType) As CompositeType

    ' TODO: Add your service operations here
    <OperationContract()>
    Function IngDataTable(ByVal ProcedureName As String, ByVal ParamArray IngFields() As String) As IngDTResult

    <OperationContract()>
    Function IngDataTableCmdText(ByVal CommandText As String) As IngDTResult

    <OperationContract()>
    Function IngDataTableMultiProc(ByVal ParamArray ProcedureName() As String) As IngDTResult

    <OperationContract()>
    Function genTransSRTNoNew(noOfSRT As Integer, tranMatrix As String) As IngDTResult

    <OperationContract()>
    Function IngDataTableMultiProcWithTKT(ProcedureName() As String, tktProcedureName As String, tktParams As String()) As IngDTResult

End Interface

' Use a data contract as illustrated in the sample below to add composite types to service operations.

<DataContract()>
Public Class CompositeType

    <DataMember()>
    Public Property BoolValue() As Boolean

    <DataMember()>
    Public Property StringValue() As String

End Class

<DataContract()>
Public Structure IngDTResult

    <DataMember()>
    Dim isDataGet As Boolean

    <DataMember()>
    Dim DataSetResult As DataSet

    <DataMember()>
    Dim DTErrorMsg As String

End Structure

