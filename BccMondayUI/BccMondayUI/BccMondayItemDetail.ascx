<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BccMondayItemDetail.ascx.cs" Inherits="RockWeb.Plugins.com_baysideonline.BccMondayUI.BccMondayItemDetail" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>

        <style type="text/css">
            .copy-link-area {
                background-color: lightgray;
                color: inherit;
                cursor: default !important;
                user-select: all !important;
            }

            .iconBounce {
                color: white;
                animation: bounce 1.5s;
                animation-iteration-count: 2;
            }

            @keyframes bounce {
                0%, 25%, 50%, 75%, 100% {
                    transform: translateY(0);
                }
                15% {
                    transform: translateY(3px);
                }
                40% {
                    transform: translateY(-9px);
                }
                60% {
                    transform: translateY(-5px);
                }
            }
        </style>

        <div id="dAlertError" class="alert alert-danger" runat="server">
            <asp:Literal ID="lError" runat="server"/>
        </div>
        <asp:Panel ID="pnlItem" CssClass="panel panel-block" runat="server">
            <div class="panel-heading bcc-monday-panel-heading row">
                <div class="col-xs-6 p-0">
                    <h1 class="panel-title d-flex flex-column justify-content-center">
                        <div>
                            <asp:Literal ID="lActionTitle" runat="server" />
                        </div>
                        <div>
                            <span ID="sCreatedAt" class="text-muted" style="font-size: .8rem;" runat="server"></span>
                        </div>
                    </h1>
                </div>
                <div class="col-xs-6 text-right p-0">
                    <Rock:BootstrapButton ID="bbtnApprove" OnClick="bbtnApprove_Click" CssClass="btn btn-secondary" runat="server">Approve Request</Rock:BootstrapButton>
                    <Rock:BootstrapButton ID="bbtnClose" OnClick="bbtnClose_Click" CssClass="btn btn-secondary" runat="server">Close Request</Rock:BootstrapButton>
                </div>
            </div>

            <div class="panel-body" style="width:100%">
                <div class="bcc-monday-column-group">
                    <div class="d-flex mb-3 flex-wrap" style="justify-content: space-between">
                        <asp:Repeater ID="rptColumns" OnItemDataBound="rptColumns_ItemDataBound" runat="server">
                            <ItemTemplate>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>

                <div class="mb-3">
                    <Rock:RockTextBox ID="tbNewUpdate" Label="New Update" TextMode="MultiLine" Rows="5" CssClass="mb-2" Placeholder="Write a new update..." Visible="false" runat="server" />
                    <Rock:BootstrapButton ID="bbtnNewUpdateOpen" OnClick="bbtnNewUpdateOpen_Click" Visible ="true" CssClass="btn btn-primary w-100" runat="server" >New Update</Rock:BootstrapButton>
                    <div style="display: flex; align-items: flex-start; justify-content: space-between;flex-direction: row-reverse;">
                        <div>
                            <Rock:BootstrapButton ID="bbtnNewUpdateSave" OnClick="bbtnNewUpdateSave_Click" Visible="false" CssClass="btn btn-primary" runat="server" >Save</Rock:BootstrapButton>
                            <Rock:BootstrapButton ID="bbtnNewUpdateCancel" OnClick="bbtnNewUpdateCancel_Click" Visible="false" CssClass="btn btn-secondary" runat="server" >Cancel</Rock:BootstrapButton>
                        </div>
                        <div>
                            <Rock:FileUploader ID="fsFile" OnFileUploaded="fsFile_FileUploaded" OnFileRemoved="fsFile_FileRemoved"
                        Visible="false" Enabled="true" ShowDeleteButton="true" UploadAsTemporary="true" AllowMultipleUploads="false" runat="server" />
                        </div>
                    </div>
                </div>

                <asp:Repeater ID="rptUpdates" OnItemCommand="rptUpdates_ItemCommand" OnItemDataBound="rptUpdates_ItemDataBound" runat="server">
                    <ItemTemplate>
                        <asp:Panel ID="mondayUpdate" class="bcc-monday-well-group" runat="server">
                            <div class="well" runat="server">
                                <p><%#Eval("TextBody")%></p>
                                <div>
                                    <span class="font-weight-bold"><%#Eval("CreatorName")%></span>
                                    <span>on <%# Eval("CreatedAt") %></span>
                                </div>
                                <asp:Repeater ID="rptFiles" DataSource="<%# GetFiles(Container.DataItem) %>" runat="server">
                                    <ItemTemplate>
                                        <a href="<%# DataBinder.Eval(Container.DataItem, "PublicUrl") %>">
                                            <%# DataBinder.Eval(Container.DataItem, "Name") %>
                                        </a>
                                        <br />
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                            <asp:Repeater ID="rptReplies" DataSource="<%# GetReplies(Container.DataItem) %>" runat="server">
                                <ItemTemplate>
                                    <div class="row d-flex flex-nowrap align-items-center">
                                        <i class="fa fa-reply fa-rotate-180 fa-2x m-2 d-inline-block"></i>
                                        <div class="well d-inline-block">
                                            <p><%# DataBinder.Eval(Container.DataItem, "TextBody") %></p>
                                            <div>
                                                <span style="word-break:break-all" class="font-weight-bold"><%# DataBinder.Eval(Container.DataItem, "CreatorName") %></span>
                                                <span>on <%# DataBinder.Eval(Container.DataItem, "CreatedAt") %></span>
                                            </div>
                                        </div>
                                   </div>
                                </ItemTemplate>
                            </asp:Repeater>
                            <div class="row">
                                <div class="col-md-12 d-flex mb-2 p-0 mt-2">
                                    <div class="d-flex align-items-center">
                                        <i class="fa fa-reply fa-rotate-180 fa-2x m-2"></i>
                                    </div>
                                    <div class="d-flex flex-column justify-content-center w-100">
                                        <Rock:RockTextBox ID="tbNewReply" TextMode="MultiLine" Rows="5" CssClass="mb-2" Placeholder="Write a new reply..." Visible="false" runat="server" />
                                        <div>
                                            <Rock:BootstrapButton ID="bbtnNewReplyOpen" CommandName="ReplyOpen" CssClass="btn btn-secondary" runat="server">New Reply</Rock:BootstrapButton>
                                            <Rock:BootstrapButton ID="bbtnNewReplySave" CommandName="ReplySave" Visible="false" CssClass="btn btn-primary" runat="server">Save</Rock:BootstrapButton>
                                            <Rock:BootstrapButton ID="bbtnNewReplyCancel" CommandName="ReplyCancel" Visible="false" CssClass="btn btn-secondary" runat="server">Cancel</Rock:BootstrapButton>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </asp:Panel>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </asp:Panel>

        <Rock:ModalAlert ID="maUpdateComment" runat="server" />

        <script>
            $(document).ready(function () {
                // Initialize ToolTips
                $('.js.tooltip').tooltip();

                // Preparing copy to clipboard function
                $('.js-copy').click(function () {
                    var text = $(this).attr('data-copy');
                    var el = $(this);
                    copyToClipboard(text, el);
                });
            });

            function copyToClipboard(text, el) {
                var elOriginalText = el.attr('data-original-title');

                var copyTextArea = document.createElement("textarea");
                copyTextArea.value = text;
                document.body.appendChild(copyTextArea);
                copyTextArea.select();

                var copyExec = document.execCommand('copy');
                var msg = copyExec ? 'Copied!' : 'Not Copied';
                el.attr('data-original-title', msg).tooltip('show');

                document.body.removeChild(copyTextArea);
                el.attr('data-original-title', elOriginalText);
            };

            function iconBounceFunction(iconElementId) {
                var iconElement = document.getElementById(iconElementId);
                iconElement.classList.add("iconBounce");

                setTimeout(function () {
                    iconElement.classList.remove("iconBounce");
                }, 3000);
            }

            function btnToIconFunction(elementId) {
                var iconElementId = elementId.concat('icon');
                iconBounceFunction(iconElementId);
            }
        </script>       
    </ContentTemplate>  
</asp:UpdatePanel>
