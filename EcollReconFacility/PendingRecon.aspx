<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="PendingRecon.aspx.vb" Inherits="EcollReconFacility.PendingRecon" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <%@ Reference VirtualPath="~/Site.Master" %>

    <div class="container sb-content shadow">
        <div class="row mt-2">
            <div class ="col">
                <asp:UpdatePanel ID="upRecon" runat="server">
                    <ContentTemplate>
                        <div class="row m-2">
                            <div class="col mx-auto">
                                <div class="row m-2">
                                    <div class="text-center">
                                        <span class="h4 font-weight-bold">Pending Credit Date Lines</span>
                                    </div>
                                </div>
                                <hr />
                                <div class="row m-2">
                                    <Label Class="font-weight-bold col-sm-3 col-form-label text-right">Collecting Partner</Label>
                                    <div class="col-sm-6">
                                        <asp:DropDownList ID="ddlBankInsti" runat="server" AutoPostBack="true"
                                            OnSelectedIndexChanged="OnSelectedIndexChanged" ClientIDMode="Static"
                                            CssClass="form-control b-radius border border-primary"/>
                                    </div>
                                    <div class="col-sm-3">
                                        <button type="button" class="btn btn-primary mx-auto d-block" id="btnShowCreditLineModal"
                                             data-toggle="modal" data-target="#creditLineModal"
                                            >Add Credit Date Line</button> 
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col mx-auto">
                                <asp:GridView CssClass="table table-info tblcreditlines beta" 
                                    Visible="true" ShowHeaderWhenEmpty="true" 
                                    id="gvCreditLines" runat="server" AutoGenerateColumns="false" EnableViewState="true" 
                                    OnRowDeleting="gvCreditLines_RowDeleting">
                                    <Columns>
                                        <asp:BoundField DataField="rownumber" HeaderText="#" ItemStyle-BackColor="#fefefa" />
                                        <asp:BoundField DataField="creditid" HeaderText="credit id" HeaderStyle-CssClass="d-none" ItemStyle-CssClass="d-none"/>
                                        <asp:BoundField DataField="bankinsti" HeaderText="Collecting Partner" 
                                            ItemStyle-BackColor="#fefefa" />
                                        <asp:BoundField DataField="creditdate" HeaderText="Credit Date" ItemStyle-BackColor="#fefefa"/>
                                        <asp:BoundField DataField="transdates" HeaderText="Transaction Date" ItemStyle-BackColor="#fefefa" />
                                        <asp:BoundField DataField="amtcredited" HeaderText="Amount Credited" ItemStyle-HorizontalAlign="Right"
                                            ItemStyle-BackColor="#fefefa" />
                                        <asp:BoundField DataField="amtonfile" HeaderText="Amount on File" 
                                            ItemStyle-HorizontalAlign="Right" ItemStyle-BackColor="#fefefa" />
                                        <asp:BoundField DataField="credittype" HeaderText="Credit Line Type" 
                                            ItemStyle-BackColor="#fefefa" />
                                        <asp:BoundField DataField="status" HeaderText="Status" 
                                            ItemStyle-BackColor="#fefefa"/>
                            
                                        <asp:TemplateField ItemStyle-CssClass="text-center" ItemStyle-BackColor="#fefefa">
                                            <ItemTemplate>

                                            </ItemTemplate>
                                        </asp:TemplateField>
                            
                                        <asp:TemplateField ItemStyle-BackColor="#fefefa">
                                            <Itemtemplate>
                                                <asp:LinkButton ID="lblDelete" runat="server" CommandName="Delete" 
                                                    OnClientClick="return confirm('Do you want to delete this credit line?');" Text="Delete" />
                                            </Itemtemplate>

                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col mx-auto">
                                <div class="text-left">
                                    <asp:Label CssClass="small" ID="lbltotalAmtPerCreditPeriod" runat="server" 
                                        Text=""/>
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>

    <div Class="modal hide fade" tabindex="-1" role="dialog" id="creditLineModal" aria-labelledby="creditLineModalLabel" aria-hidden="true">
        <div Class="modal-dialog modal-lg">
            <div Class="modal-content">
                <div Class="modal-header text-light" style="background:#002D62;">
                    <h4 Class="modal-title">Add New Credit Date Line</h4>
                </div>
                <div Class="modal-body" style="background:#f6f6f6;">
                    <div class="container-fluid">
                        <div class="row">
                            <div class="col-10">
                                <div class="row mb-2">
                                    <label class="col-3 col-form-label font-weight-bold">Collecting Partner</label>
                                    <div class="col-9">
                                        <asp:DropDownList ID="ddlModalBankInsti" 
                                            runat="server" CssClass="form-control border border-primary b-radius"/>
                                    </div>
                                </div>
                                <div class="row mb-2">
                                    <label class="col-3 col-form-label font-weight-bold">Credit Date</label>
                                    <div class="col-9">
                                        <asp:TextBox ID="dtpmodalCreditDate" runat="server" 
                                             ClientIDMode ="Static" 
                                            CssClass="form-control datepicker border border-primary b-radius"
                                             required="required"
                                            />
                                    </div>
                                </div>
                                <div class="row mb-2">
                                    <label class="col-3 col-form-label font-weight-bold">Transaction Date</label>
                                    <div class="col-9">
                                        <div class="row">
                                            <div class="col-6">
                                                <div class="form-group">
                                                    <label class="col-form-label">From</label>
                                                        <asp:TextBox ID="dtpTransDateFrom" runat="server" 
                                                             ClientIDMode ="Static" 
                                                            CssClass="form-control datepicker border border-primary b-radius"
                                                             required="required"
                                                            />
                                                </div>
                                            </div>
                                            <div class="col-6">
                                                <div class="form-group">
                                                    <label class="col-form-label">To</label>
                                                        <asp:TextBox ID="dtpTransDateTo" runat="server" 
                                                             ClientIDMode ="Static" 
                                                            CssClass="form-control datepicker border border-primary b-radius"
                                                             required="required"
                                                            />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="row mb-2">
                                    <label class="col-3 col-form-label font-weight-bold">Amount Credited</label>
                                    <div class="col-9">
                                        <asp:TextBox ID="txtAmountCredited" runat="server" ClientIDMode="Static"
                                            CssClass="form-control border border-primary b-radius font-weight-bold"
                                            required="required" />

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer" style="background:#F6F6F6;">       
                    <asp:Label ID="lblMessage" ForeColor="#f00000" runat="server" ClientIDMode="Static" />
                    <asp:Button ID="btnModalSave" CssClass="btn btn-primary" OnClick="btnModalSave_Click" 
                        OnClientClick="return checkCreditLineFields();" Text="Save" runat="server" />
                    <button type="button" id="btnHideCreditLineModal" class="btn btn-danger" onclick="$('#creditLineModal').modal('hide');">Cancel</button>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
