<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="UCMonitoring.aspx.vb" Inherits="EcollReconFacility.UCMonitoring" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container sb-content">
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
                                    OnSelectedIndexChanged="OnSelectedIndexChanged" 
                                    CssClass="form-control b-radius border border-primary"/>
                            </div>
                        </div>
                        <div class="row m-3">
                            <div class="col-sm-3">
                                <label class="font-weight-bold col-form-label text-right">Reconciliation Type</label>
                            </div>
                            <div class="col-sm-9">
                                <asp:DropDownList ID="ddlReconType" runat="server" AutoPostBack="true"
                                    OnSelectedIndexChanged="OnSelectedIndexChanged" 
                                    CssClass="form-control b-radius border border-primary">
                                    <asp:ListItem Text="UC (Unidentified Collections)" Value="UC"/>
                                    <asp:ListItem Text="AR (Accounts Receivable)" Value="AR"/>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="row m-3">
                            <div class="col-sm-3">
                                <label class="font-weight-bold col-form-label text-right">Status</label>
                            </div>
                            <div class="col-sm-9">
                                <asp:DropDownList ID="ddlReconStatus" runat="server" AutoPostBack="true"
                                    OnSelectedIndexChanged="OnSelectedIndexChanged" 
                                    CssClass="form-control b-radius border border-primary">
                                    <asp:ListItem Text="PENDING" Value="PENDING"/>
                                    <asp:ListItem Text="CLOSED" Value="CLOSED"/>
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col mx-auto">
                                <asp:GridView ID="gvUC" CssClass="table table-primary tblcreditlines beta tbluc"
                                    ShowHeaderWhenEmpty ="true" runat="server" AutoGenerateColumns ="false"
                                        AllowPaging ="true" PageSize ="10" EnableViewState ="true">
                                    <Columns>                                        
                                        <asp:BoundField DataField="rownumber" HeaderText="#" />
                                        <asp:BoundField DataField ="creditid" HeaderText="credit id" HeaderStyle-CssClass="d-none" ItemStyle-CssClass="d-none"/>
                                        <asp:BoundField DataField ="reconno" HeaderText="recon no" HeaderStyle-CssClass="d-none" ItemStyle-CssClass="d-none"/>
                                        <asp:BoundField DataField="vardate" HeaderText="Recon Date" />
                                        <asp:BoundField DataField="varamount" HeaderText="UC Amount" ItemStyle-HorizontalAlign="Right"/>
                                        <asp:BoundField DataField="status" HeaderText="Status" />

                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkReconcile" Text="Reconcile" runat="server" OnClientClick="" ></asp:LinkButton>
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
                <div Class="modal-header bg-primary text-light">
                    <h4 Class="modal-title">Reconcile UC/AR</h4>
                </div>
                <div Class="modal-body" style="background:#E0FFFF;">
                    <div class="container-fluid">
                        <div class="row">
                            <div class="col-10 mx-auto">
                                <div class="row mb-2">
                                    <label class="col-3 col-form-label font-weight-bold">Selected Row No.</label>
                                    <div class="col-8">
                                        <asp:TextBox ID="txtRowNo"
                                             CssClass="form-control border border-primary b-radius" runat="server"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row mb-2">
                                    <label class="col-3 col-form-label font-weight-bold">Collecting Partner</label>
                                    <div class="col-8">
                                        <asp:TextBox ID="txtBankInsti"
                                             CssClass="form-control border border-primary b-radius" runat="server"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row mb-2">
                                    <label class="col-3 col-form-label font-weight-bold">Reconciliation Type</label>
                                    <div class="col-8">
                                        <asp:TextBox ID="txtReconType"
                                            CssClass="form-control border border-primary b-radius" runat="server"/>
                                    </div>
                                </div>
                                <div class="row mb-2">
                                    <label class="col-3 col-form-label font-weight-bold">Amount Credited</label>
                                    <div class="col-8">
                                        <input type="text" id="txtAmountCredited" 
                                            class="form-control border border-primary b-radius font-weight-bold" runat="server"/>
                                     
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer" style="background:#E0FFFF;">         
                    <asp:Button ID="btnModalSave" CssClass="btn btn-primary" Text="Save" runat="server"/>
                    <button type="button" id="btnHideModal" class="btn btn-danger" onclick="$('#reconModal').modal('hide');">Cancel</button>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
