System.register(['vue', '@Obsidian/Utility/block', '@Obsidian/Controls/rockButton', '@Obsidian/Controls/panel'], (function (exports) {
  'use strict';
  var createElementVNode, defineComponent, openBlock, createElementBlock, Fragment, toDisplayString, ref, inject, renderList, createBlock, unref, createCommentVNode, createTextVNode, resolveComponent, withCtx, createVNode, useConfigurationValues, useInvokeBlockAction, useReloadBlock, onConfigurationValuesChanged, RockButton, Panel;
  return {
    setters: [function (module) {
      createElementVNode = module.createElementVNode;
      defineComponent = module.defineComponent;
      openBlock = module.openBlock;
      createElementBlock = module.createElementBlock;
      Fragment = module.Fragment;
      toDisplayString = module.toDisplayString;
      ref = module.ref;
      inject = module.inject;
      renderList = module.renderList;
      createBlock = module.createBlock;
      unref = module.unref;
      createCommentVNode = module.createCommentVNode;
      createTextVNode = module.createTextVNode;
      resolveComponent = module.resolveComponent;
      withCtx = module.withCtx;
      createVNode = module.createVNode;
    }, function (module) {
      useConfigurationValues = module.useConfigurationValues;
      useInvokeBlockAction = module.useInvokeBlockAction;
      useReloadBlock = module.useReloadBlock;
      onConfigurationValuesChanged = module.onConfigurationValuesChanged;
    }, function (module) {
      RockButton = module["default"];
    }, function (module) {
      Panel = module["default"];
    }],
    execute: (function () {

      var _hoisted_1$2 = ["href", "textContent"];
      var _hoisted_2$2 = createElementVNode("br", null, null, -1);
      var script$2 = defineComponent({
        name: 'mondayFile.partial',
        props: {
          file: {
            type: Object
          }
        },
        setup(__props) {
          return (_ctx, _cache) => {
            return openBlock(), createElementBlock(Fragment, null, [createElementVNode("a", {
              href: __props.file.publicUrl,
              textContent: toDisplayString(__props.file.name)
            }, null, 8, _hoisted_1$2), _hoisted_2$2], 64);
          };
        }
      });

      script$2.__file = "src/mondayItemDetail/mondayFile.partial.obs";

      var _hoisted_1$1 = {
        id: "mondayUpdate",
        class: "bcc-monday-well-group"
      };
      var _hoisted_2$1 = {
        class: "well"
      };
      var _hoisted_3$1 = ["textContent"];
      var _hoisted_4$1 = ["textContent"];
      var _hoisted_5$1 = ["textContent"];
      var script$1 = defineComponent({
        name: 'mondayUpdate.partial',
        props: {
          update: {
            type: Object,
            required: true
          }
        },
        setup(__props) {
          ref(false);
          var item = inject("mondayItemBag");
          item.updates;
          return (_ctx, _cache) => {
            return openBlock(), createElementBlock("div", _hoisted_1$1, [createElementVNode("div", _hoisted_2$1, [createElementVNode("p", {
              textContent: toDisplayString(__props.update.textBody)
            }, null, 8, _hoisted_3$1), createElementVNode("div", null, [createElementVNode("span", {
              class: "font-weight-bold",
              textContent: toDisplayString(__props.update.creatorName)
            }, null, 8, _hoisted_4$1), createElementVNode("span", {
              textContent: toDisplayString(__props.update.createdAt)
            }, null, 8, _hoisted_5$1)]), (openBlock(true), createElementBlock(Fragment, null, renderList(__props.update.files, file => {
              return openBlock(), createBlock(unref(script$2), {
                file: file
              }, null, 8, ["file"]);
            }), 256))]), (openBlock(true), createElementBlock(Fragment, null, renderList(__props.update.replies, reply => {
              return openBlock(), createElementBlock(Fragment, {
                key: reply.Id
              }, [createCommentVNode(" <MondayReply :reply-=\"reply\"></MondayReply> ")], 64);
            }), 128))]);
          };
        }
      });

      script$1.__file = "src/mondayItemDetail/mondayUpdate.partial.obs";

      var _hoisted_1 = {
        class: "panel-heading bcc-monday-panel-heading row"
      };
      var _hoisted_2 = {
        class: "col-xs-6 p-0"
      };
      var _hoisted_3 = {
        class: "panel-title d-flex flex-column justify-content-center"
      };
      var _hoisted_4 = ["textContent"];
      var _hoisted_5 = ["textContent"];
      var _hoisted_6 = {
        class: "col-xs-6 text-right p-0"
      };
      var _hoisted_7 = createTextVNode("Approve Request");
      var _hoisted_8 = createTextVNode("Close Request");
      var _hoisted_9 = {
        class: "panel-body",
        style: {
          "width": "100%"
        }
      };
      var _hoisted_10 = {
        class: "bcc-monday-column-group"
      };
      var _hoisted_11 = {
        class: "d-flex mb-3 flex-wrap",
        style: {
          "justify-content": "space-between"
        }
      };
      var _hoisted_12 = {
        class: "mb-3"
      };
      var _hoisted_13 = createTextVNode("New Update");
      var _hoisted_14 = {
        style: {
          "display": "flex",
          "align-items": "flex-start",
          "justify-content": "space-between",
          "flex-direction": "row-reverse"
        }
      };
      var _hoisted_15 = createTextVNode("Save");
      var _hoisted_16 = createTextVNode("Cancel");
      var _hoisted_17 = createElementVNode("div", null, [createElementVNode("div", null, "File Uploader Here")], -1);
      var script = exports('default', defineComponent({
        name: 'mondayItemDetail',
        setup(__props) {
          ref(false);
          var config = useConfigurationValues();
          useInvokeBlockAction();
          var reloadBlock = useReloadBlock();
          ref(false);
          ref("");
          ref("");
          ref("");
          var item = config;
          onConfigurationValuesChanged(reloadBlock);
          return (_ctx, _cache) => {
            var _component_FileColumnValue = resolveComponent("FileColumnValue");
            var _component_StatusColumnValue = resolveComponent("StatusColumnValue");
            var _component_BasicColumnValue = resolveComponent("BasicColumnValue");
            return openBlock(), createBlock(unref(Panel), {
              id: "pnlItem",
              class: "panel panel-block"
            }, {
              default: withCtx(() => [createElementVNode("div", _hoisted_1, [createElementVNode("div", _hoisted_2, [createElementVNode("h1", _hoisted_3, [createElementVNode("div", {
                textContent: toDisplayString(unref(item).name)
              }, null, 8, _hoisted_4), createElementVNode("div", null, [createElementVNode("span", {
                class: "text-muted",
                style: {
                  "font-size": "0.8rem"
                },
                textContent: toDisplayString(unref(item).createdAt)
              }, null, 8, _hoisted_5)])])]), createElementVNode("div", _hoisted_6, [createVNode(unref(RockButton), {
                id: "bbtnApprove",
                class: "btn btn-secondary"
              }, {
                default: withCtx(() => [_hoisted_7]),
                _: 1
              }), createVNode(unref(RockButton), {
                id: "bbtnClose",
                class: "btn btn-secondary"
              }, {
                default: withCtx(() => [_hoisted_8]),
                _: 1
              })])]), createElementVNode("div", _hoisted_9, [createElementVNode("div", _hoisted_10, [createElementVNode("div", _hoisted_11, [(openBlock(true), createElementBlock(Fragment, null, renderList(unref(item).columnValues, columnValue => {
                return openBlock(), createElementBlock(Fragment, {
                  key: columnValue.ColumnId
                }, [columnValue.type == 'file' ? (openBlock(), createBlock(_component_FileColumnValue, {
                  key: 0
                })) : columnValue.type == 'status' ? (openBlock(), createBlock(_component_StatusColumnValue, {
                  key: 1
                })) : (openBlock(), createBlock(_component_BasicColumnValue, {
                  key: 2
                }))], 64);
              }), 128))])]), createElementVNode("div", _hoisted_12, [createCommentVNode(" <TextBox id=\"tbNewUpdate\" textMode=\"multiline\" class=\"mb-2\" placeholder=\"Write a new update...\"></TextBox> "), createVNode(unref(RockButton), {
                id: "bbtnNewUpdateOpen",
                class: "btn btn-primary w-100"
              }, {
                default: withCtx(() => [_hoisted_13]),
                _: 1
              }), createElementVNode("div", _hoisted_14, [createElementVNode("div", null, [createVNode(unref(RockButton), {
                id: "bbtnNewUpdateSave",
                class: "btn btn-primary"
              }, {
                default: withCtx(() => [_hoisted_15]),
                _: 1
              }), createVNode(unref(RockButton), {
                id: "bbtnNewUpdateCancel",
                class: "btn btn-secondary"
              }, {
                default: withCtx(() => [_hoisted_16]),
                _: 1
              })]), _hoisted_17])]), (openBlock(true), createElementBlock(Fragment, null, renderList(unref(item).updates, update => {
                return openBlock(), createBlock(unref(script$1), {
                  key: update.Id,
                  update: update
                }, null, 8, ["update"]);
              }), 128))])]),
              _: 1
            });
          };
        }
      }));

      script.__file = "src/mondayItemDetail.obs";

    })
  };
}));
//# sourceMappingURL=mondayItemDetail.obs.js.map
