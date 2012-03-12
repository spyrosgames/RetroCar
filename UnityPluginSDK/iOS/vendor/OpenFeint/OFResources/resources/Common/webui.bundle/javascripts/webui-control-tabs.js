// DO NOT EDIT
//   Generated from javascripts/webui-control-tabs.coffee
//
(function() {
  var Tab, TabControl;
  var __bind = function(fn, me){ return function(){ return fn.apply(me, arguments); }; };
  OF.control = {};
  Tab = (function() {
    function Tab(name, path, options, active, controller, contentElement, tabElement) {
      this.name = name;
      this.path = path;
      this.options = options;
      this.active = active;
      this.controller = controller;
      this.contentElement = contentElement;
      this.tabElement = tabElement;
      this.pageStack = OF.pageStackController.addStack(this.contentElement, this.name);
      this.isLoaded = false;
    }
    Tab.prototype.bindTouchHandler = function() {
      return $(this.tabElement).touch(__bind(function() {
        return this.controller.setActiveTab(this.name);
      }, this));
    };
    Tab.prototype.goBack = function(options) {
      if (options == null) {
        options = {};
      }
      this.pageStack.back(options);
      return this.pageStack.length === 0;
    };
    Tab.prototype.replace = function(path, options) {
      this.path = path;
      if (options == null) {
        options = {};
      }
      this.pageStack.replace(this.path, options);
      return this.isLoaded = true;
    };
    Tab.prototype.activate = function(options) {
      if (options == null) {
        options = this.options;
      }
      if (!this.isLoaded) {
        this.load(options);
      }
      this.controller.showTab(this);
      return this.pageStack.activate();
    };
    Tab.prototype.load = function(options) {
      if (options == null) {
        options = {};
      }
      this.pageStack.push(this.path, options);
      return this.isLoaded = true;
    };
    return Tab;
  })();
  TabControl = (function() {
    function TabControl(options) {
      var callback, contentElement, index, newTab, tab, _i, _j, _len, _len2, _len3, _ref, _ref2, _ref3;
      this.options = options;
      /* expects:
      {
        tabs:[
          {name:"tab name", path:"flow/page", active:bool},
          ...
        ],
        location: jquery obj
        native:bool (default: false)
      }
      */
      this.tabs = [];
      _ref = this.options.tabs;
      for (_i = 0, _len = _ref.length; _i < _len; _i++) {
        tab = _ref[_i];
        contentElement = this.getElementName();
        this.options.location.append($("<div id='" + contentElement + "'></div>"));
        newTab = new Tab(tab.name, tab.path, tab.options, tab.active, this, "#" + contentElement, tab.element);
        $.extend(newTab, tab);
        newTab.bindTouchHandler();
        this.tabs.push(newTab);
      }
      this.activeTabIndex = 0;
      _ref2 = this.tabs;
      for (index = 0, _len2 = _ref2.length; index < _len2; index++) {
        tab = _ref2[index];
        if (tab.active) {
          this.activeTabIndex = index;
        }
      }
      this.location = this.options.location;
      this.tabs[this.activeTabIndex].activate({
        params: OF.page.params
      });
      if (options["native"]) {
        this.nativeClient();
      }
      _ref3 = ['beforeSwitch', 'afterSwitch'];
      for (_j = 0, _len3 = _ref3.length; _j < _len3; _j++) {
        callback = _ref3[_j];
        if (!(callback in this.options)) {
          this.options[callback] = $.noop;
        }
      }
    }
    TabControl.prototype.getElementName = function(tabInput) {
      return "tab" + (Math.floor(Math.random() * 10000));
    };
    TabControl.prototype.setActiveTab = function(name) {
      var index, tab, _len, _ref;
      _ref = this.tabs;
      for (index = 0, _len = _ref.length; index < _len; index++) {
        tab = _ref[index];
        if (tab.name === name) {
          this.activeTabIndex = index;
        }
      }
      return this.tabs[this.activeTabIndex].activate();
    };
    TabControl.prototype.setActiveTabByIndex = function(index) {
      var activeTab, pagePushOptions;
      this.activeTabIndex = index;
      activeTab = this.tabs[index];
      pagePushOptions = this.options.beforeSwitch(index, activeTab);
      activeTab.activate(pagePushOptions);
      return this.options.afterSwitch(index, activeTab);
    };
    TabControl.prototype.showTab = function(tab) {
      var hidetabs, _i, _len, _ref;
      _ref = this.tabs;
      for (_i = 0, _len = _ref.length; _i < _len; _i++) {
        hidetabs = _ref[_i];
        $(hidetabs.contentElement).css({
          'display': 'none'
        });
      }
      return $(tab.contentElement).css({
        'display': 'block'
      });
    };
    TabControl.prototype.goBack = function(options) {
      if (options == null) {
        options = {};
      }
      return this.tabs[this.activeTabIndex].goBack(options);
    };
    TabControl.prototype.nativeClient = function() {
      var nativeConfig, nativeTabOptionOf, tab, tabs;
      if (OF.action.isSupported('configureNativeTabs')) {
        OF.tabCallback = __bind(function(index) {
          return this.setActiveTabByIndex(index);
        }, this);
        nativeConfig = this.options.nativeConfig || {};
        nativeTabOptionOf = function(tab) {
          return {
            name: tab.name,
            activeImage: tab.activeImage,
            inactiveImage: tab.inactiveImage
          };
        };
        tabs = (function() {
          var _i, _len, _ref, _results;
          _ref = this.tabs;
          _results = [];
          for (_i = 0, _len = _ref.length; _i < _len; _i++) {
            tab = _ref[_i];
            _results.push(nativeTabOptionOf(tab));
          }
          return _results;
        }).call(this);
        return OF.action('configureNativeTabs', {
          activeBackground: nativeConfig.activeBackground,
          inactiveBackground: nativeConfig.inactiveBackground,
          leftDivider: nativeConfig.leftDivider,
          rightDivider: nativeConfig.leftDivider,
          callback: function(tabindex) {
            OF.log("SELECTED TAB " + tabindex);
            return OF.tabCallback(tabindex);
          },
          height: nativeConfig.height,
          selectedTabName: this.tabs[this.activeTabIndex].name,
          tabs: tabs
        });
      }
    };
    return TabControl;
  })();
  OF.control.TabControl = TabControl;
}).call(this);
