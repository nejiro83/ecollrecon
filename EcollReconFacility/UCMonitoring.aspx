<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="UCMonitoring.aspx.vb" Inherits="EcollReconFacility.UCMonitoring" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container sb-content shadow">
        <div class="row mt-2">
            <div class="col mx-auto m-2">
                <asp:UpdatePanel ID="upMonitoring" runat="server">
                    <ContentTemplate>
                        <div class="row m-3">
                            <div class="col-sm-3">
                                <label class="font-weight-bold col-form-label text-right">Collecting Partner</label>
                            </div>
                            <div class="col-sm-9">
                                <asp:DropDownList ID="ddlBankInsti" runat="server" AutoPostBack="true"
                                    OnSelectedIndexChanged="OnSelectedIndexChanged" ClientIDMode="Static"
                                    CssClass="form-control b-radius border border-primary"/>
                            </div>
                        </div>
                        <div class="row m-3">
                            <div class="col-sm-3">
                                <label class="font-weight-bold col-form-label text-right">Reconciliation Type</label>
                            </div>
                            <div class="col-sm-9">
                                <asp:DropDownList ID="ddlReconType" runat="server" AutoPostBack="true"
                                    OnSelectedIndexChanged="OnSelectedIndexChanged" ClientIDMode="Static"
                                    CssClass="form-control b-radius border border-primary">
                                    <asp:ListItem Text="UC (Unidentified Collections)" Value="UC"/>
                                    <asp:ListItem Text="AR (Accounts Receivable)" Value="AR"/>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="row m-3">
                            <div class="col-sm-4">
                                <div class="form-group">
                                    <label class="col-form-label font-weight-bold">Month</label>
                                    <asp:DropDownList ID="ddlReconMonth" EnableViewState="true" runat="server" 
                                        CssClass ="form-control border border-primary b-radius"
                                        OnSelectedIndexChanged="OnSelectedIndexChanged" AutoPostBack="true">
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
                                        OnSelectedIndexChanged="OnSelectedIndexChanged" AutoPostBack="true"
                                        CssClass="form-control border border-primary b-radius"/>
                                </div>
                            </div>
                            <div class="col-sm-4">
                                <div class="form-group">
                                    <label class="col-form-label font-weight-bold">Status</label>
                                    <asp:DropDownList ID="ddlReconStatus" runat="server" 
                                         OnSelectedIndexChanged="OnSelectedIndexChanged" AutoPostBack="true"
                                        CssClass="form-control border border-primary b-radius">
                                        <asp:ListItem Text="PENDING" Value="RP"/>
                                        <asp:ListItem Text="CLOSED" Value="RX"/>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col mx-auto">
                                <asp:GridView ID="gvUC" CssClass="table table-primary tblcreditlines beta"
                                    ShowHeaderWhenEmpty ="true" runat="server" AutoGenerateColumns ="false"
                                        AllowPaging ="true" PageSize ="10" EnableViewState ="true">
                                    <Columns>                                        
                                        <asp:BoundField DataField="rownumber" HeaderText="#" />
                                        <asp:BoundField DataField ="creditid" HeaderText="credit id" HeaderStyle-CssClass="d-none" ItemStyle-CssClass="d-none"/>
                                        <asp:BoundField DataField ="reconno" HeaderText="recon no" HeaderStyle-CssClass="d-none" ItemStyle-CssClass="d-none"/>
                                        <asp:BoundField DataField="vardate" HeaderText="Recon Date" />
                                        <asp:BoundField DataField="varamount" HeaderText="Amount" ItemStyle-HorizontalAlign="Right"/>
                                        <asp:BoundField DataField="status" HeaderText="Status" />

                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkReconcile" Text="Reconcile" runat="server" 
                                                    OnClientClick=<%# "reconDetails('" +
                                                                                Eval("rownumber") + "','" +
                                                                                Eval("varamount") + "','" +
                                                                                Eval("creditid") + "','" +
                                                                                Eval("reconno") + "','" +
                                                                                ddlReconType.SelectedValue + "');"  %>></asp:LinkButton>

                                            </ItemTemplate>
                                        </asp:TemplateField>

                                    </Columns>
                                    <PagerSettings Mode="Numeric" PageButtonCount="10" />
                                </asp:GridView>
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>

    <div Class="modal hide fade" tabindex="-1" role="dialog" id="reconModal" aria-labelledby="reconModalLabel" aria-hidden="true">
        <div Class="modal-dialog modal-lg">
            <div Class="modal-content">
                <div Class="modal-header text-light" style="background:#002D62;">
                    <h4 Class="modal-title">Reconcile AR</h4>
                </div>
                <div Class="modal-body" style="background:#f6f6f6;">
                    <div class="container-fluid">
                        <div class="row">
                            <div class="col-10 mx-auto">
                                <div class="row mb-2">
                                    <label class="col-3 col-form-label font-weight-bold">Selected Row No.</label>
                                    <div class="col-8">
                                        <asp:TextBox ID="txtRowNo" ClientIDMode="Static" ReadOnly="true"
                                             CssClass="form-control border border-primary b-radius" runat="server"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row mb-2">
                                    <label class="col-3 col-form-label font-weight-bold">Collecting Partner</label>
                                    <div class="col-8">
                                        <asp:TextBox ID="txtBankInsti" ClientIDMode="Static" ReadOnly="true"
                                             CssClass="form-control border border-primary b-radius" runat="server"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row mb-2">
                                    <label class="col-3 col-form-label font-weight-bold">Amount Credited</label>
                                    <div class="col-8">
                                        <input type="text" id="txtAmountCredited" 
                                            class="form-control border border-primary b-radius font-weight-bold" 
                                            runat="server" required="required"/>   
                                        <asp:HiddenField ID="txtCreditID" ClientIDMode="Static" runat="server" />
                                        <asp:HiddenField ID="txtUCARNo" ClientIDMode="Static" runat="server" />
                                        <asp:HiddenField ID="txtUCARAmount" ClientIDMode="Static" runat="server" />
                                        <asp:HiddenField ID="txtBankInstiCode" ClientIDMode="Static" runat="server" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer" style="background:#f6f6f6;">         
                    <asp:Button ID="btnModalSave" CssClass="btn btn-primary" Text="Save" runat="server"/>
                    <button type="button" id="btnHideModal" class="btn btn-danger" onclick="$('#reconModal').modal('hide');">Cancel</button>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
