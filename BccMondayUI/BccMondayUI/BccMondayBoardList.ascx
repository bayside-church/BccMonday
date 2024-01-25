<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BccMondayBoardList.ascx.cs" Inherits="RockWeb.Plugins.com_baysideonline.BccMondayUI.BccMondayBoardList" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>

        <asp:Panel ID="pnlView" runat="server" CssClass="panel panel-block">
        
            <div class="panel-heading">
                <h1 class="panel-title">
                    <i class="fa fa-list"></i> 
                    <asp:Literal ID="lTitle" Text="Monday.com Boards" runat="server"></asp:Literal>
                </h1>
            </div>
            <div class="panel-body">

                <div class="grid grid-panel">
                    <Rock:Grid ID="gBoardList" OnRowSelected="gBoardList_RowSelected" DataKeyNames="Id" runat="server">
                        <EmptyDataTemplate>
                            <p>No Boards</p>
                        </EmptyDataTemplate>
                        <Columns>
                            <Rock:RockBoundField DataField="MondayBoardName" HeaderText="Name" SortExpression="MondayBoardName" />
                        </Columns>
                    </Rock:Grid>
                </div>

            </div>
        
        </asp:Panel>

    </ContentTemplate>
</asp:UpdatePanel>