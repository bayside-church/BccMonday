<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BccMondayBoardDetail.ascx.cs" Inherits="RockWeb.Plugins.com_baysideonline.BccMondayUI.BccMondayBoardDetail" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>

        <style type="text/css">
            .entity-type-picker .control-wrapper {
                width: 100%;
            }
        </style>

        <div ID="dAlertError" class="alert alert-danger" Visible="false" runat="server">
            <asp:Literal ID="lError" runat="server" />
        </div>

        <Rock:ModalAlert ID="maWarning" Visible="false" runat="server"></Rock:ModalAlert>

        <asp:Panel ID="pnlView" runat="server" CssClass="panel panel-block">
        
            <div class="panel-heading bcc-monday-panel-heading">
                <h1 class="panel-title">
                    <i class="fa fa-list"></i> 
                    <asp:Literal ID="lTitle" runat="server"></asp:Literal>
                </h1>
            </div>

            <div class="panel-body">

                <div id="pnlViewDetails" runat="server">
                    <div class="row">
                        <div class="col-md-6">
                            <dl>
                                <dt>Name</dt>
                                <dd>
                                    <asp:Literal ID="lMondayBoardName" runat="server" />
                                </dd>
                            </dl>
                        </div>
                        <div class="col-md-6">
                            <dl>
                                <dt>Monday Board Id</dt>
                                <dd>
                                    <asp:Literal ID="lMondayBoardId" runat="server" />
                                </dd>
                            </dl>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-6">
                            <dl>
                                <dt>Email Match Column Id</dt>
                                <dd>
                                    <asp:Literal ID="lEmailMatchColumnId" runat="server" />
                                </dd>
                            </dl>
                        </div>
                        <div class="col-md-6">
                            <dl>
                                <dt>Status Column Id</dt>
                                <dd>
                                    <asp:Literal ID="lStatusColumnId" runat="server" />
                                </dd>
                            </dl>
                        </div>
                        <div class="col-md-6">
                            <dl>
                                <dt>Complete Status Column Value</dt>
                                <dd>
                                    <asp:Literal ID="lCompleteStatusValue" runat="server" />
                                </dd>
                            </dl>
                        </div>
                        <div class="col-md-6">
                            <dl>
                                <dt>Closed Status Column Value</dt>
                                <dd>
                                    <asp:Literal ID="lClosedStatusValue" runat="server" />
                                </dd>
                            </dl>
                        </div>
                        <div class="col-md-6">
                            <dl>
                                <dt>Approved Status Column Value</dt>
                                <dd>
                                    <asp:Literal ID="lApprovedStatusValue" runat="server" />
                                    <asp:Literal ID="lShowApprove" runat="server" />
                                </dd>
                            </dl>
                        </div>
                        <div class="col-md-6">
                            <dl>
                                <dt>Displayed Columns</dt>
                                <dd>
                                    <asp:Literal ID="lDisplayColumns" runat="server" />
                                </dd>
                            </dl>
                        </div>
                        <div class="col-md-6">
                            <dl>
                                <dt>Workspace</dt>
                                <dd>
                                    <asp:Literal ID="lWorkSpace" runat="server" />
                                </dd>
                            </dl>
                        </div>
                        <div class="col-md-12">
                            <Rock:AttributeValuesContainer ID="avcAttributes" runat="server" />
                        </div>
                    </div>

                    <div class="actions">
                        <asp:LinkButton ID="lbEdit" Text="Edit" CssClass="btn btn-primary" OnClick="lbEdit_Click" CausesValidation="false" runat="server" />
                        <asp:LinkButton ID="lbDelete" Text="Delete" CssClass="btn btn-link" OnClick="lbDelete_Click" CausesValidation="false" runat="server" />

                        <div class="pull-right">
                            <asp:Button ID="btnSync" OnClick="btnSync_Click" Text="Sync" runat="server" />
                        </div>
                    </div>
                </div>

                <div id="pnlEditDetails" Visible="false" runat="server">
                    <div class="row">
                        <div class="col-md-6">
                            <Rock:DataTextBox ID="dtbMondayBoardName" SourceTypeName="com.baysideonline.BccMonday.Models.BccMondayBoard, BccMonday"
                                PropertyName="MondayBoardName" Required="false" Label="Name" runat="server" />
                        </div>
                        <div class="col-md-6">
                            <Rock:RockDropDownList ID="ddlMondayBoardId"
                                DataTextField="Name" DataValueField="Id" Label="Monday.com Boards"
                                ValidationGroup="" OnSelectedIndexChanged="ddlMondayBoardId_SelectedIndexChanged" 
                                EnhanceForLongLists="true" AutoPostBack="true" runat="server"/>
                            <Rock:RockTextBox ID="tbEditMondayBoardId" Label="Monday.com Board Id" Visible="false" runat="server" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-6">
                            <Rock:RockDropDownList ID="ddlEmailMatchColumnId"
                                DataTextField="Title" DataValueField="Id" Label="Email Match Column"
                                Required="true"
                                ValidationGroup="" EnhanceForLongLists="true" runat="server" />
                        </div>
                        <div class="col-md-6">
                            <Rock:RockDropDownList ID="ddlMondayStatusColumnId"
                                DataTextField="Title" DataValueField="Id" Label="Status Column"
                                ValidationGroup="" OnSelectedIndexChanged="ddlMondayStatusColumnId_SelectedIndexChanged" 
                                 AutoPostBack="true" Required="true"
                                EnhanceForLongLists="true" runat="server" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-6">
                            <Rock:RockDropDownList ID="ddlMondayStatusCompleteValue" 
                                ValidationGroup=""
                                EnhanceForLongLists="true" Required="true" Label="Complete Status Value" runat="server" />
                        </div>
                        <div class="col-md-6">
                            <Rock:RockDropDownList ID="ddlMondayStatusClosedValue" 
                                ValidationGroup=""
                                EnhanceForLongLists="true" Required="true" Label="Closed Status Value" runat="server" />
                        </div>
                        <div class="col-md-6">
                            <Rock:RockDropDownList ID="ddlMondayStatusApprovedValue" 
                            ValidationGroup=""
                            EnhanceForLongLists="true" Label="Approved Status Value" runat="server" />
                        </div>
                        <div class="col-md-6">
                            <Rock:RockCheckBox ID="cbShowApprove" Label="Show Approve Button?" runat="server" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <Rock:AttributeValuesContainer ID="avcAttributesEdit" runat="server" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <div class="grid">
                                <Rock:Grid ID="gEditDisplayColumns" DisplayType="Light" ShowHeader="true" RowItemText="Column" runat="server">
                                    <Columns>
                                        <Rock:RockBoundField DataField="MondayColumnTitle" HeaderText="Title"/>
                                        <Rock:RockBoundField DataField="MondayColumnId" HeaderText="Id" />
                                        <Rock:DeleteField OnClick="gEditDisplayColumns_Delete" />
                                    </Columns>
                                </Rock:Grid>
                            </div>
                        </div>
                    </div>

                    <Rock:ModalAlert ID="modalAlert" runat="server" />

                    <Rock:ModalDialog ID="dlgDisplayColumns" OnSaveClick="dlgDisplayColumns_SaveClick" ValidationGroup="" runat="server">
                        <Content>
                            <Rock:RockListBox ID="lbDisplayColumns" DataTextField="Title" DataValueField="Id" Label="Display Columns" ValidationGroup="" runat="server" />
                        </Content>
                    </Rock:ModalDialog>

                    <div class="actions">
                        <asp:LinkButton ID="lbSave" Text="Save" CssClass="btn btn-primary" OnClick="lbSave_Click" runat="server" />
                        <asp:LinkButton ID="lbCancel" Text="Cancel" CssClass="btn btn-link" OnClick="lbCancel_Click" CausesValidation="false" runat="server" />
                    </div>
                </div>
                
            </div>
        
        </asp:Panel>

    </ContentTemplate>
</asp:UpdatePanel>