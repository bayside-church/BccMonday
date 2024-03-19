System.register(['vue', '@Obsidian/Utility/block', '@Obsidian/Controls/rockButton.obs', '@Obsidian/Controls/textBox.obs'], (function (exports) {
  'use strict';
  var createElementVNode, defineComponent, openBlock, createElementBlock, toDisplayString, Fragment, createTextVNode, inject, ref, renderList, createBlock, unref, createCommentVNode, withCtx, onMounted, watch, normalizeStyle, provide, createVNode, useInvokeBlockAction, useConfigurationValues, useReloadBlock, onConfigurationValuesChanged, RockButton, TextBox;
  return {
    setters: [function (module) {
      createElementVNode = module.createElementVNode;
      defineComponent = module.defineComponent;
      openBlock = module.openBlock;
      createElementBlock = module.createElementBlock;
      toDisplayString = module.toDisplayString;
      Fragment = module.Fragment;
      createTextVNode = module.createTextVNode;
      inject = module.inject;
      ref = module.ref;
      renderList = module.renderList;
      createBlock = module.createBlock;
      unref = module.unref;
      createCommentVNode = module.createCommentVNode;
      withCtx = module.withCtx;
      onMounted = module.onMounted;
      watch = module.watch;
      normalizeStyle = module.normalizeStyle;
      provide = module.provide;
      createVNode = module.createVNode;
    }, function (module) {
      useInvokeBlockAction = module.useInvokeBlockAction;
      useConfigurationValues = module.useConfigurationValues;
      useReloadBlock = module.useReloadBlock;
      onConfigurationValuesChanged = module.onConfigurationValuesChanged;
    }, function (module) {
      RockButton = module["default"];
    }, function (module) {
      TextBox = module["default"];
    }],
    execute: (function () {

      function asyncGeneratorStep(gen, resolve, reject, _next, _throw, key, arg) {
        try {
          var info = gen[key](arg);
          var value = info.value;
        } catch (error) {
          reject(error);
          return;
        }
        if (info.done) {
          resolve(value);
        } else {
          Promise.resolve(value).then(_next, _throw);
        }
      }
      function _asyncToGenerator(fn) {
        return function () {
          var self = this,
            args = arguments;
          return new Promise(function (resolve, reject) {
            var gen = fn.apply(self, args);
            function _next(value) {
              asyncGeneratorStep(gen, resolve, reject, _next, _throw, "next", value);
            }
            function _throw(err) {
              asyncGeneratorStep(gen, resolve, reject, _next, _throw, "throw", err);
            }
            _next(undefined);
          });
        };
      }

      var _hoisted_1$7 = {
        class: "row d-flex flex-nowrap align-items-center"
      };
      var _hoisted_2$7 = createElementVNode("i", {
        class: "fa fa-reply fa-rotate-180 fa-2x m-2 d-inline-block"
      }, null, -1);
      var _hoisted_3$5 = {
        class: "well d-inline-block"
      };
      var _hoisted_4$5 = ["textContent"];
      var _hoisted_5$3 = ["textContent"];
      var _hoisted_6$2 = ["textContent"];
      var script$7 = defineComponent({
        name: 'mondayReply.partial',
        props: {
          reply: {
            type: Object,
            required: true
          }
        },
        setup(__props) {
          return (_ctx, _cache) => {
            return openBlock(), createElementBlock("div", _hoisted_1$7, [_hoisted_2$7, createElementVNode("div", _hoisted_3$5, [createElementVNode("p", {
              textContent: toDisplayString(__props.reply.textBody)
            }, null, 8, _hoisted_4$5), createElementVNode("div", null, [createElementVNode("span", {
              style: {
                "word-break": "break-all"
              },
              class: "font-weight-bold",
              textContent: toDisplayString(__props.reply.creatorName)
            }, null, 8, _hoisted_5$3), createElementVNode("span", {
              textContent: toDisplayString(' on ' + __props.reply.createdAt)
            }, null, 8, _hoisted_6$2)])])]);
          };
        }
      });

      script$7.__file = "src/mondayItemDetail/mondayReply.partial.obs";

      var _hoisted_1$6 = ["href", "textContent"];
      var _hoisted_2$6 = createElementVNode("br", null, null, -1);
      var script$6 = defineComponent({
        name: 'mondayFile.partial',
        props: {
          file: {
            type: Object,
            required: true
          }
        },
        setup(__props) {
          return (_ctx, _cache) => {
            return openBlock(), createElementBlock(Fragment, null, [createElementVNode("a", {
              href: __props.file.publicUrl,
              textContent: toDisplayString(__props.file.name)
            }, null, 8, _hoisted_1$6), _hoisted_2$6], 64);
          };
        }
      });

      script$6.__file = "src/mondayItemDetail/mondayFile.partial.obs";

      var _hoisted_1$5 = {
        id: "mondayUpdate",
        class: "bcc-monday-well-group"
      };
      var _hoisted_2$5 = {
        class: "well"
      };
      var _hoisted_3$4 = ["textContent"];
      var _hoisted_4$4 = ["textContent"];
      var _hoisted_5$2 = ["textContent"];
      var _hoisted_6$1 = {
        class: "row"
      };
      var _hoisted_7$1 = {
        class: "col-md-12 d-flex mb-2 p-0 mt-2"
      };
      var _hoisted_8$1 = createElementVNode("div", {
        class: "d-flex align-items-center"
      }, [createElementVNode("i", {
        class: "fa fa-reply fa-rotate-180 fa-2x m-2"
      })], -1);
      var _hoisted_9$1 = {
        class: "d-flex flex-column justify-content-center w-100"
      };
      var _hoisted_10$1 = createTextVNode("New Reply");
      var _hoisted_11$1 = createTextVNode("Save");
      var _hoisted_12$1 = createTextVNode("Cancel");
      var script$5 = defineComponent({
        name: 'mondayUpdate.partial',
        props: {
          update: {
            type: Object,
            required: true
          }
        },
        setup(__props) {
          var props = __props;
          var invokeBlockAction = useInvokeBlockAction();
          var getItemDetailArgs = inject("getItemDetailArgs");
          var newReply = ref("");
          var newReplyOpen = ref(false);
          function toggleVisibility() {
            newReplyOpen.value = !newReplyOpen.value;
            newReply.value = "";
          }
          function saveReply() {
            return _saveReply.apply(this, arguments);
          }
          function _saveReply() {
            _saveReply = _asyncToGenerator(function* () {
              var replyText = newReply.value;
              var args = {
                args: getItemDetailArgs(),
                text: replyText,
                updateId: props.update.id
              };
              var response = yield invokeBlockAction("SaveReply", args);
              if (response.data) {
                var _props$update$replies;
                var reply = response.data;
                if (props.update.replies === null) {
                  props.update.replies = [];
                }
                (_props$update$replies = props.update.replies) === null || _props$update$replies === void 0 || _props$update$replies.push(reply);
              }
              toggleVisibility();
            });
            return _saveReply.apply(this, arguments);
          }
          return (_ctx, _cache) => {
            return openBlock(), createElementBlock("div", _hoisted_1$5, [createElementVNode("div", _hoisted_2$5, [createElementVNode("p", {
              textContent: toDisplayString(__props.update.textBody)
            }, null, 8, _hoisted_3$4), createElementVNode("div", null, [createElementVNode("span", {
              class: "font-weight-bold",
              textContent: toDisplayString(__props.update.creatorName)
            }, null, 8, _hoisted_4$4), createElementVNode("span", {
              textContent: toDisplayString(' on ' + __props.update.createdAt)
            }, null, 8, _hoisted_5$2)]), (openBlock(true), createElementBlock(Fragment, null, renderList(__props.update.files, file => {
              return openBlock(), createBlock(unref(script$6), {
                file: file
              }, null, 8, ["file"]);
            }), 256))]), (openBlock(true), createElementBlock(Fragment, null, renderList(__props.update.replies, reply => {
              return openBlock(), createBlock(unref(script$7), {
                key: reply.Id,
                reply: reply
              }, null, 8, ["reply"]);
            }), 128)), createElementVNode("div", _hoisted_6$1, [createElementVNode("div", _hoisted_7$1, [_hoisted_8$1, createElementVNode("div", _hoisted_9$1, [newReplyOpen.value ? (openBlock(), createBlock(unref(TextBox), {
              key: 0,
              id: "tbNewReply",
              modelValue: newReply.value,
              "onUpdate:modelValue": _cache[0] || (_cache[0] = $event => newReply.value = $event),
              textMode: "multiline",
              class: "mb-2",
              placeholder: "Write a new reply..."
            }, null, 8, ["modelValue"])) : createCommentVNode("v-if", true), createElementVNode("div", null, [!newReplyOpen.value ? (openBlock(), createBlock(unref(RockButton), {
              key: 0,
              id: "bbtnNewReplyOpen",
              onclick: toggleVisibility,
              class: "btn btn-secondary"
            }, {
              default: withCtx(() => [_hoisted_10$1]),
              _: 1
            })) : createCommentVNode("v-if", true), newReplyOpen.value ? (openBlock(), createBlock(unref(RockButton), {
              key: 1,
              id: "bbtnNewReplySave",
              onClick: saveReply,
              class: "btn btn-primary"
            }, {
              default: withCtx(() => [_hoisted_11$1]),
              _: 1
            })) : createCommentVNode("v-if", true), newReplyOpen.value ? (openBlock(), createBlock(unref(RockButton), {
              key: 2,
              id: "bbtnNewReplyCancel",
              onClick: toggleVisibility,
              class: "btn btn-secondary"
            }, {
              default: withCtx(() => [_hoisted_12$1]),
              _: 1
            })) : createCommentVNode("v-if", true)])])])])]);
          };
        }
      });

      script$5.__file = "src/mondayItemDetail/mondayUpdate.partial.obs";

      var _hoisted_1$4 = {
        class: "mr-4"
      };
      var _hoisted_2$4 = {
        class: "font-weight-bold"
      };
      var _hoisted_3$3 = {
        style: {
          "min-width": "200px"
        }
      };
      var _hoisted_4$3 = ["href"];
      var script$4 = defineComponent({
        name: 'fileColumnValue.partial',
        props: {
          columnValue: {
            type: Object,
            required: true
          }
        },
        setup(__props) {
          return (_ctx, _cache) => {
            return openBlock(), createElementBlock("div", _hoisted_1$4, [createElementVNode("p", _hoisted_2$4, toDisplayString(__props.columnValue.column.title), 1), createElementVNode("div", _hoisted_3$3, [(openBlock(true), createElementBlock(Fragment, null, renderList(__props.columnValue.files, file => {
              return openBlock(), createElementBlock("div", {
                key: file.assetId
              }, [createElementVNode("a", {
                href: file.asset.publicUrl
              }, toDisplayString(file.asset.name), 9, _hoisted_4$3)]);
            }), 128))])]);
          };
        }
      });

      script$4.__file = "src/mondayItemDetail/fileColumnValue.partial.obs";

      var _hoisted_1$3 = {
        class: "mr-4"
      };
      var _hoisted_2$3 = {
        class: "font-weight-bold"
      };
      var _hoisted_3$2 = {
        class: "bcc-monday-column-empty",
        style: {
          'min-width': '200px'
        }
      };
      var _hoisted_4$2 = {
        class: "btn-group"
      };
      var _hoisted_5$1 = {
        class: "monday-btn",
        style: {
          borderRadius: '6px',
          backgroundColor: 'lightgray'
        }
      };
      var script$3 = defineComponent({
        name: 'basicColumnValue.partial',
        props: {
          columnValue: {
            type: Object,
            required: true
          }
        },
        setup(__props) {
          var props = __props;
          var columnText = ref(props.columnValue.text !== "" ? props.columnValue.text : "Unknown");
          ref(null);
          ref(false);
          var isContentOverflowing = ref(false);
          var checkContentOverflow = () => {
            isContentOverflowing.value = columnText.value.length > 40;
          };
          onMounted(() => {
            checkContentOverflow();
          });
          watch(columnText, () => {
            checkContentOverflow();
          });
          return (_ctx, _cache) => {
            return openBlock(), createElementBlock(Fragment, null, [createCommentVNode("\r\n  <div v-show=\"columnText.length > 20\" class=\"box-details\" ref=\"cardRef\">\r\n    <div class=\"box-info\">Information</div>\r\n    <div :class=\"{ 'box-text': true, 'truncate-card-information': true, expanded: isExpanded }\" v-show=\"!isContentOverflowing || isExpanded\">\r\n      <div v-if=\"isContentOverflowing\" class=\"truncate-text-overlay\"></div>\r\n      {{ columnText }}\r\n    </div>\r\n    <button v-show=\"isContentOverflowing\" class=\"show-button\" @click=\"toggleExpansion\">\r\n      {{ isExpanded ? 'Show Less' : 'Show More...' }}\r\n    </button>\r\n  </div>"), createElementVNode("div", _hoisted_1$3, [createElementVNode("p", _hoisted_2$3, toDisplayString(__props.columnValue.column.title), 1), createElementVNode("div", _hoisted_3$2, [createElementVNode("div", _hoisted_4$2, [createElementVNode("div", _hoisted_5$1, [createElementVNode("span", null, toDisplayString(columnText.value), 1)])])])])], 2112);
          };
        }
      });

      function styleInject(css, ref) {
        if (ref === void 0) ref = {};
        var insertAt = ref.insertAt;
        if (!css || typeof document === 'undefined') {
          return;
        }
        var head = document.head || document.getElementsByTagName('head')[0];
        var style = document.createElement('style');
        style.type = 'text/css';
        if (insertAt === 'top') {
          if (head.firstChild) {
            head.insertBefore(style, head.firstChild);
          } else {
            head.appendChild(style);
          }
        } else {
          head.appendChild(style);
        }
        if (style.styleSheet) {
          style.styleSheet.cssText = css;
        } else {
          style.appendChild(document.createTextNode(css));
        }
      }

      var css_248z$1 = ".truncate-card-information{max-height:150px;overflow:hidden;position:relative;transition:max-height .3s ease-in-out}.truncate-card-information .truncate-text-overlay{background:linear-gradient(180deg,transparent 0,#383d47);height:100%;left:0;opacity:.8;position:absolute;top:0;transition:opacity .3s ease;width:100%}.truncate-card-information.expanded{max-height:unset}.truncate-card-information.expanded .truncate-text-overlay{display:none}.show-button{background-color:transparent;border:none;color:#00aada;cursor:pointer;display:block}.monday-btn{background-image:none;border:1px solid transparent;border-radius:6px;display:inline-block;font-size:16px;font-weight:500;line-height:1.5;margin-bottom:0;padding:6px 16px;text-align:center;vertical-align:middle;white-space:nowrap}";
      styleInject(css_248z$1);

      script$3.__file = "src/mondayItemDetail/basicColumnValue.partial.obs";

      var _hoisted_1$2 = {
        class: "mr-4"
      };
      var _hoisted_2$2 = {
        class: "font-weight-bold"
      };
      var script$2 = defineComponent({
        name: 'statusColumnValue.partial',
        props: {
          columnValue: {
            type: Object,
            required: true
          }
        },
        setup(__props) {
          return (_ctx, _cache) => {
            var _props$columnValue$l;
            return openBlock(), createElementBlock("div", null, [createElementVNode("div", _hoisted_1$2, [createElementVNode("p", _hoisted_2$2, toDisplayString(__props.columnValue.column.title), 1), createElementVNode("div", {
              class: "text-center",
              style: normalizeStyle({
                'background-color': (_props$columnValue$l = __props.columnValue.labelStyle) === null || _props$columnValue$l === void 0 ? void 0 : _props$columnValue$l.color,
                'text-overflow': 'ellipsis',
                'white-space': 'nowrap',
                'overflow': 'hidden',
                'padding': '11px 22px',
                'min-width': '200px',
                'max-width': '250px',
                'color': '#ffffff',
                'border-radius': '10px'
              })
            }, [createElementVNode("span", null, toDisplayString(__props.columnValue.text || 'Unknown'), 1)], 4)])]);
          };
        }
      });

      script$2.__file = "src/mondayItemDetail/statusColumnValue.partial.obs";

      var _hoisted_1$1 = {
        class: "mr-4"
      };
      var _hoisted_2$1 = {
        class: "font-weight-bold"
      };
      var _hoisted_3$1 = {
        style: {
          "min-width": "200px"
        }
      };
      var _hoisted_4$1 = ["href"];
      var script$1 = defineComponent({
        name: 'boardRelationColumnValue.partial',
        props: {
          columnValue: {
            type: Object,
            required: true
          }
        },
        setup(__props) {
          function generateUrl(itemId) {
            var currentUrl = window.location.href;
            var urlWithoutQuery = currentUrl.split('?')[0];
            var url = "".concat(urlWithoutQuery, "?MondayItemId=").concat(itemId);
            return url;
          }
          return (_ctx, _cache) => {
            return openBlock(), createElementBlock("div", _hoisted_1$1, [createElementVNode("p", _hoisted_2$1, toDisplayString(__props.columnValue.column.title), 1), createElementVNode("div", _hoisted_3$1, [(openBlock(true), createElementBlock(Fragment, null, renderList(__props.columnValue.linkedItemIds, linkedItemId => {
              return openBlock(), createElementBlock("div", {
                key: linkedItemId
              }, [createElementVNode("a", {
                href: generateUrl(linkedItemId)
              }, "Item", 8, _hoisted_4$1)]);
            }), 128))])]);
          };
        }
      });

      script$1.__file = "src/mondayItemDetail/boardRelationColumnValue.partial.obs";

      var _hoisted_1 = {
        id: "pnlItem",
        class: "panel panel-block"
      };
      var _hoisted_2 = {
        class: "panel-heading"
      };
      var _hoisted_3 = {
        class: "col-xs-6 p-0"
      };
      var _hoisted_4 = {
        class: "panel-title d-flex flex-column justify-content-center"
      };
      var _hoisted_5 = ["textContent"];
      var _hoisted_6 = ["textContent"];
      var _hoisted_7 = {
        class: "header-actions col-xs-6 text-right p-0"
      };
      var _hoisted_8 = {
        class: "panel-body",
        style: {
          "width": "100%"
        }
      };
      var _hoisted_9 = {
        class: "bcc-monday-column-group"
      };
      var _hoisted_10 = {
        class: "d-flex mb-3 flex-wrap",
        style: {
          "justify-content": "space-between"
        }
      };
      var _hoisted_11 = {
        class: "mb-3"
      };
      var _hoisted_12 = {
        style: {
          "display": "flex",
          "align-items": "flex-start",
          "justify-content": "space-between",
          "flex-direction": "row-reverse"
        }
      };
      var script = exports('default', defineComponent({
        name: 'mondayItemDetail',
        setup(__props) {
          var config = useConfigurationValues();
          var invokeBlockAction = useInvokeBlockAction();
          var reloadBlock = useReloadBlock();
          console.log("Config from block:\n ", config);
          var item = config.item;
          var newUpdate = ref("");
          var newUpdateOpen = ref(false);
          var fileUploadValue = ref();
          var approveVisible = ref(config.showApprove);
          var closeVisible = ref(config.showClose);
          var statusColumnValue = ref(item.columnValues[config.statusIndex]);
          var getItemDetailArgs = () => {
            return {
              mondayItemId: item.id,
              mondayBoardId: item.board.id
            };
          };
          provide("getItemDetailArgs", getItemDetailArgs);
          var toggleVisibility = () => {
            newUpdateOpen.value = !newUpdateOpen.value;
            newUpdate.value = "";
            fileUploadValue.value = undefined;
          };
          function saveUpdate() {
            return _saveUpdate.apply(this, arguments);
          }
          function _saveUpdate() {
            _saveUpdate = _asyncToGenerator(function* () {
              var updateText = newUpdate.value;
              console.log(updateText);
              var blockActionArgs = {
                args: getItemDetailArgs(),
                text: updateText
              };
              var response = yield invokeBlockAction("SaveUpdate", blockActionArgs);
              if (response.data) {
                var update = response.data;
                item.updates.unshift(update);
              }
              newUpdate.value = "";
              newUpdateOpen.value = !newUpdateOpen.value;
            });
            return _saveUpdate.apply(this, arguments);
          }
          function changeColumnValue(_x) {
            return _changeColumnValue.apply(this, arguments);
          }
          function _changeColumnValue() {
            _changeColumnValue = _asyncToGenerator(function* (statusChange) {
              var args = {
                args: getItemDetailArgs(),
                statusChange: statusChange
              };
              var response = yield invokeBlockAction("ChangeColumnValue", args);
              if (response.data) {
                var _response$data = response.data,
                  color = _response$data.color,
                  statusText = _response$data.statusText;
                approveVisible.value = showStatusButton(statusText, config.showApprove);
                closeVisible.value = statusChange !== 'Close';
                item.columnValues[config.statusIndex].text = statusText;
                item.columnValues[config.statusIndex].labelStyle = {
                  color: color,
                  border: ""
                };
              }
            });
            return _changeColumnValue.apply(this, arguments);
          }
          function showStatusButton(status, showApprove) {
            var currentStatus = item.columnValues[config.statusIndex].text;
            var foo = currentStatus !== status && (showApprove === undefined || showApprove);
            console.log(foo, currentStatus, status);
            return foo;
          }
          onConfigurationValuesChanged(reloadBlock);
          return (_ctx, _cache) => {
            var _statusColumnValue$va;
            return openBlock(), createElementBlock("div", _hoisted_1, [createElementVNode("div", _hoisted_2, [createElementVNode("div", _hoisted_3, [createElementVNode("h1", _hoisted_4, [createElementVNode("div", {
              textContent: toDisplayString(unref(item).name)
            }, null, 8, _hoisted_5), createElementVNode("div", null, [createElementVNode("span", {
              class: "text-muted",
              style: {
                "font-size": "0.8rem"
              },
              textContent: toDisplayString(unref(item).createdAt)
            }, null, 8, _hoisted_6)])])]), createElementVNode("div", _hoisted_7, [approveVisible.value ? (openBlock(), createBlock(unref(RockButton), {
              key: 0,
              id: "bbtnApprove",
              class: "btn btn-secondary",
              onClick: _cache[0] || (_cache[0] = $event => changeColumnValue('Approve')),
              textContent: 'Approve Request'
            })) : createCommentVNode("v-if", true), closeVisible.value ? (openBlock(), createBlock(unref(RockButton), {
              key: 1,
              id: "bbtnClose",
              class: "btn btn-secondary",
              onClick: _cache[1] || (_cache[1] = $event => changeColumnValue('Close')),
              textContent: 'Close Request'
            })) : createCommentVNode("v-if", true)])]), createElementVNode("div", {
              class: "w-100",
              style: normalizeStyle({
                'background-color': (_statusColumnValue$va = statusColumnValue.value.labelStyle) === null || _statusColumnValue$va === void 0 ? void 0 : _statusColumnValue$va.color,
                textAlign: 'center',
                color: '#ffffff',
                padding: '11px 22px'
              })
            }, toDisplayString(statusColumnValue.value.text), 5), createElementVNode("div", _hoisted_8, [createElementVNode("div", _hoisted_9, [createElementVNode("div", _hoisted_10, [(openBlock(true), createElementBlock(Fragment, null, renderList(unref(item).columnValues, (columnValue, index) => {
              return openBlock(), createElementBlock(Fragment, {
                key: columnValue.ColumnId
              }, [index !== unref(config).statusIndex ? (openBlock(), createElementBlock(Fragment, {
                key: 0
              }, [columnValue.type == 'file' ? (openBlock(), createBlock(unref(script$4), {
                key: 0,
                columnValue: columnValue
              }, null, 8, ["columnValue"])) : columnValue.type == 'status' ? (openBlock(), createBlock(unref(script$2), {
                key: 1,
                columnValue: columnValue
              }, null, 8, ["columnValue"])) : columnValue.type == 'board_relation' ? (openBlock(), createBlock(unref(script$1), {
                key: 2,
                columnValue: columnValue
              }, null, 8, ["columnValue"])) : (openBlock(), createBlock(unref(script$3), {
                key: 3,
                columnValue: columnValue
              }, null, 8, ["columnValue"]))], 64)) : createCommentVNode("v-if", true)], 64);
            }), 128))])]), createElementVNode("div", _hoisted_11, [!newUpdateOpen.value ? (openBlock(), createBlock(unref(RockButton), {
              key: 0,
              id: "bbtnNewUpdateOpen",
              onclick: toggleVisibility,
              class: "btn btn-primary w-100",
              textContent: 'New Update'
            })) : createCommentVNode("v-if", true), newUpdateOpen.value ? (openBlock(), createElementBlock(Fragment, {
              key: 1
            }, [createVNode(unref(TextBox), {
              id: "tbNewUpdate",
              modelValue: newUpdate.value,
              "onUpdate:modelValue": _cache[2] || (_cache[2] = $event => newUpdate.value = $event),
              textMode: "multiline",
              class: "mb-2",
              placeholder: "Write a new update..."
            }, null, 8, ["modelValue"]), createElementVNode("div", _hoisted_12, [createElementVNode("div", null, [createVNode(unref(RockButton), {
              id: "bbtnNewUpdateSave",
              onClick: saveUpdate,
              class: "btn btn-primary",
              textContent: 'Save'
            }), createVNode(unref(RockButton), {
              id: "bbtnNewUpdateCancel",
              onClick: toggleVisibility,
              class: "btn btn-secondary",
              textContent: 'Cancel'
            })]), createCommentVNode(" <div>\r\n                            <FileUploader v-model=\"fileUploadValue\" :uploadAsTemporary=\"true\" uploadButtonText=\"Upload\" :showDeleteButton=\"true\" />\r\n                        </div> ")])], 64)) : createCommentVNode("v-if", true)]), (openBlock(true), createElementBlock(Fragment, null, renderList(unref(item).updates, update => {
              return openBlock(), createBlock(unref(script$5), {
                key: update.Id,
                update: update
              }, null, 8, ["update"]);
            }), 128))])]);
          };
        }
      }));

      var css_248z = ".copy-link-area{background-color:#d3d3d3;color:inherit;cursor:default!important;user-select:all!important}.iconBounce{animation:bounce 1.5s;animation-iteration-count:2;color:#fff}@keyframes bounce{0%,25%,50%,75%,to{transform:translateY(0)}15%{transform:translateY(3px)}40%{transform:translateY(-9px)}60%{transform:translateY(-5px)}}";
      styleInject(css_248z);

      script.__file = "src/mondayItemDetail.obs";

    })
  };
}));
//# sourceMappingURL=mondayItemDetail.obs.js.map
