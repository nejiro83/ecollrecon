<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="SearchRecon.aspx.vb" Inherits="EcollReconFacility.SearchRecon" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container p-3 sb-content shadow">
        <div class="row mt-2">
            <div class="col">
                <div class="row">
                    <Label Class="font-weight-bold col-sm-3 col-form-label text-right">Collecting Partner</Label>
                    <div class="col-sm-6">
                        <asp:DropDownList ID="ddlBankInsti" runat="server" 
                            CssClass="form-control border border-primary b-radius"/>
                    </div>
                    <div class="col-sm-3">
                        <asp:Button ID="btnSearchTickets" runat="server" CssClass="btn btn-primary d-block" Text="Search" />
                    </div>
                </div>
            </div>
        </div>
        <div class="row m-2">
            <div class="col-sm-4">
                <div class="form-group">
                    <label class="col-form-label font-weight-bold">Month</label>
                    <asp:DropDownList ID="ddlReconMonth" EnableViewState="true" runat="server" 
                        CssClass ="form-control border border-primary b-radius">
                        <asp:ListItem Text ="January" Value="1"/>
                        <asp:ListItem Text ="February" Value="2"/>
                        <asp:ListItem Text ="March" Value="3"/>
                        <asp:ListItem Text ="April" Value="4"/>
                        <asp:ListItem Text ="May" Value="5"/>
                        <asp:ListItem Text ="June" Value="6"/>
                        <asp:ListItem Text ="July" Value="7"/>
                        <asp:ListItem Text ="August" Value="8"/>
                        <asp:ListItem Text ="September" Value="9"/>
                        <asp:ListItem Text ="October" Value="10"/>
                        <asp:ListItem Text ="November" Value="11"/>
                        <asp:ListItem Text ="December" Value="12"/>
                    </asp:DropDownList>
                </div>
            </div>
            <div class="col-sm-4">
                <div class="form-group">
                    <label class="col-form-label font-weight-bold">Year</label>
                    <asp:DropDownList ID="ddlReconYear" EnableViewState="true" runat="server" 
                        CssClass="form-control border border-primary b-radius"/>
                </div>
            </div>
            <div class="col-sm-4">
                <div class="form-group">
                    <label class="col-form-label font-weight-bold">Status</label>
                    <asp:DropDownList ID="ddlReconStatus" runat="server" 
                        CssClass="form-control border border-primary b-radius">
                        <asp:ListItem Text="ALL" Value="ALL"/>
                        <asp:ListItem Text="PENDING" Value="CP"/>
                        <asp:ListItem Text="PENDING (WITH AR)" Value="CA"/>
                        <asp:ListItem Text="PENDING (WITH UC)" Value="CU"/>
                        <asp:ListItem Text="CLOSED" Value="CX"/>
                    </asp:DropDownList>
                </div>
            </div>
        </div>
        <div class="row mt-2">
            <div class="col mx-auto">
                <asp:GridView CssClass="table table-primary tblcreditlines beta" Visible="true" ShowHeaderWhenEmpty="true" 
                    id="gvReconBackLog" runat="server" AutoGenerateColumns="false" EnableViewState="true"
                    OnRowDeleting="gvCreditLines_RowDeleting">
                    <Columns>
                        <asp:BoundField DataField="rownumber" HeaderText="#" />
                        <asp:BoundField DataField="creditid" HeaderText="credit id" HeaderStyle-CssClass="d-none" ItemStyle-CssClass="d-none"/>
                        <asp:BoundField DataField="creditdate" HeaderText="Credit Date" />
                        <asp:BoundField DataField="transdatefrom" HeaderText="Transaction Date - From" />
                        <asp:BoundField DataField="transdateto" HeaderText="Transaction Date - To" />
                        <asp:BoundField DataField="amtcredited" HeaderText="Amount Credited" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="amtonfile" HeaderText="Amount on File" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="credittype" HeaderText="Credit Line Type" />
                        <asp:BoundField DataField="status" HeaderText="Status" />
                    
                        <asp:TemplateField ItemStyle-CssClass="text-center">
                            <ItemTemplate>

                            </ItemTemplate>
                        
                        </asp:TemplateField>
                    
                        <asp:TemplateField>
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
            <div class="text-left">
                <asp:Label CssClass="small" ID="lblMessage" runat="server" 
                            Text=""/>
            </div>
        </div>
    </div>
</asp:Content>
