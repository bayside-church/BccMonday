<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BccMondayItemList.ascx.cs" Inherits="RockWeb.Plugins.com_baysideonline.BccMondayUI.BccMondayItemList" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>

        <div ID="dAlertError" class="alert alert-danger" Visible="false" runat="server">
            <asp:Literal ID="lError" runat="server" />
        </div>

        <asp:Panel ID="pnlLists" runat="server">

            <div class="panel panel-block">
                <div class="panel-heading">
                    <h1 class="panel-title">
                        <i class="fa fa-list"></i>
                        <asp:Literal ID="lTitle" runat="server" />
                    </h1>
                </div>
                <Rock:GridFilter ID="gfMondayList"
                    OnApplyFilterClick="gfMondayList_ApplyFilterClick"
                    OnClearFilterClick="gfMondayList_ClearFilterClick"
                    OnDisplayFilterValue="gfMondayList_DisplayFilterValue" runat="server">                
                    <Rock:RockDropDownList ID="ddlBoardOption" Label="Selected Board" runat="server" >
                        <asp:ListItem Text="Open" Value="Open"></asp:ListItem>
                        <asp:ListItem Text="Closed" Value="Closed"></asp:ListItem>
                    </Rock:RockDropDownList>
                </Rock:GridFilter>
                <Rock:Grid ID="gMondayList" DataKeyNames="Id" OnRowSelected="gMondayList_RowSelected"
                    OnRowDataBound="gMondayList_RowDataBound" AllowSorting="true" runat="server">
                    <Columns>
                        <Rock:RockBoundField DataField="Name" HeaderText="Name" SortExpression="Name"/>
                        <Rock:RockTemplateField HeaderText="Status" SortExpression="Status">
                            <ItemTemplate>
                            </ItemTemplate>
                        </Rock:RockTemplateField>
                        <Rock:DateField DataField="CreatedAt" HeaderText="Created" SortExpression="Created At"/>
                    </Columns>
                </Rock:Grid>
                 <Rock:ModalDialog ID="mdDialog" Title="This May Take a While..." SaveButtonText="Yes" OnSaveClick="mdDialog_SaveClick" runat="server" >
                    <Content>
                        <asp:ValidationSummary ID="valSummaryValue" runat="server" CssClass="alert alert-error"/>
                        <fieldset>
                            <legend>Are you sure you want to view closed items?</legend>
                        </fieldset>
                    </Content>
                </Rock:ModalDialog>
            </div>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
