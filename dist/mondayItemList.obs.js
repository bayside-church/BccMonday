System.register(['vue', '@Obsidian/Utility/block'], (function (exports) {
  'use strict';
  var defineComponent, openBlock, createElementBlock, unref, useConfigurationValues, useInvokeBlockAction, useReloadBlock, onConfigurationValuesChanged;
  return {
    setters: [function (module) {
      defineComponent = module.defineComponent;
      openBlock = module.openBlock;
      createElementBlock = module.createElementBlock;
      unref = module.unref;
    }, function (module) {
      useConfigurationValues = module.useConfigurationValues;
      useInvokeBlockAction = module.useInvokeBlockAction;
      useReloadBlock = module.useReloadBlock;
      onConfigurationValuesChanged = module.onConfigurationValuesChanged;
    }],
    execute: (function () {

      var _hoisted_1 = ["innerHTML"];
      var script = exports('default', defineComponent({
        name: 'mondayItemList',
        setup(__props) {
          var config = useConfigurationValues();
          useInvokeBlockAction();
          var reloadBlock = useReloadBlock();
          console.log("Config from block:\n ", config);
          var content = config.content;
          onConfigurationValuesChanged(reloadBlock);
          return (_ctx, _cache) => {
            return openBlock(), createElementBlock("div", {
              innerHTML: unref(content)
            }, null, 8, _hoisted_1);
          };
        }
      }));

      script.__file = "src/mondayItemList.obs";

    })
  };
}));
//# sourceMappingURL=mondayItemList.obs.js.map
