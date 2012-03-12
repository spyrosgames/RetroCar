// DO NOT EDIT
//   Generated from javascripts/webui-pagestack.coffee
//
(function() {
  var Page, PageStack, PageStackController;
  var __bind = function(fn, me){ return function(){ return fn.apply(me, arguments); }; };
  PageStackController = (function() {
    PageStackController.prototype.legacyPush = function(path, options) {
      return this.activeStack.push(path, options);
    };
    PageStackController.prototype.legacyReady = function(pageJSON) {
      var loadingStack;
      if (!(pageJSON != null)) {
        this.legacyLoadQueue[this.legacyLoadQueue.length - 1].loadingPage.ajaxLoad();
        return;
      }
      loadingStack = this.legacyLoadQueue.pop();
      this.activate(loadingStack);
      return loadingStack.ready(pageJSON);
    };
    PageStackController.prototype.legacyGoBack = function(options) {
      var waitForTransitionAnimation;
      if (options == null) {
        options = {};
      }
      if (this.activeStack.pages.length <= 1) {
        return OF.action('dismiss');
      }
      OF.touch.cancel();
      OF.action.now('back', options, function() {});
      waitForTransitionAnimation = OF.platform === 'ios' ? 350 : 0;
      return setTimeout(__bind(function() {
        var _results;
        this.activeStack.back();
        if (options.root) {
          _results = [];
          while (this.activeStack.pages.length > 1) {
            _results.push(this.activeStack.back());
          }
          return _results;
        }
      }, this), waitForTransitionAnimation);
    };
    PageStackController.prototype.legacyRefresh = function() {
      return this.activeStack.activePage.refresh();
    };
    PageStackController.prototype.legacyIPushed = function(stack) {
      OF.init.isLoaded = false;
      return this.legacyLoadQueue.push(stack);
    };
    function PageStackController() {
      this.legacyLoadQueue = [];
      this.stacks = [];
      this.loadedFlows = {};
      this.activeStack = null;
    }
    PageStackController.prototype.createDefaultStack = function() {
      OF.pageStackController.addStack('#page');
      return this.activate(this.stacks[0]);
    };
    PageStackController.prototype.addStack = function(element, stackId) {
      var newStack;
      newStack = new PageStack(element, this, stackId);
      this.stacks.push(newStack);
      return newStack;
    };
    PageStackController.prototype.getStack = function(index) {
      return this.stacks[index];
    };
    PageStackController.prototype.activate = function(stack) {
      this.activeStack = stack;
      OF.pages = this.activeStack.pages;
      return OF.pages.replace = __bind(function(url, options) {
        return this.activeStack.replace(url, options);
      }, this);
    };
    PageStackController.prototype.isFlowLoaded = function(flow) {
      return this.loadedFlows[flow] != null;
    };
    PageStackController.prototype.markFlowAsLoaded = function(flow) {
      console.error('Loading flow:', flow);
      return this.loadedFlows[flow] = true;
    };
    PageStackController.prototype.browser = function() {
      if (OF.isBrowser && $('#browser_toolbar').length === 0) {
        $.loadCss('browser_toolbar', false);
        return $.get('browser_toolbar.html', function(data) {
          return $(document.body).append(data);
        });
      }
    };
    PageStackController.prototype.registerAsGlobal = function(name) {
      return this.globalName = name;
    };
    return PageStackController;
  })();
  PageStack = (function() {
    function PageStack(element, controller, stackId) {
      this.element = element;
      this.controller = controller;
      this.stackId = stackId;
      this.pages = [];
      this.loadingPage = null;
      this.activePage = null;
    }
    PageStack.prototype.push = function(url, options) {
      var _ref;
      if (options == null) {
        options = {};
      }
      if (this.loadingPage != null) {
        return;
      }
      this.controller.legacyIPushed(this);
      url = $.jsonifyUrl(url);
      options.path = url;
      OF.log("Loading content: " + url);
      this.loadingPage = new Page(url, options, this);
      this.pages.push(this.loadingPage);
      this.loadingPage.startLoading();
      OF.loader.show();
      if ((_ref = this.activePage) != null) {
        _ref.scrollPosition = window.scrollY;
      }
      if (options.complete) {
        options.complete();
      }
      return OF.specs.load(url);
    };
    PageStack.prototype.replace = function(url, options) {
      while (this.pages.length > 0) {
        this.back();
      }
      return this.push(url, options);
    };
    PageStack.prototype.back = function(options) {
      if (!(this.pages.length <= 1)) {
        this.detach();
        this.pages.pop();
        this.activePage = this.pages[this.pages.length - 1];
        this.updateLegacyPage();
        return this.show();
      } else {
        return OF.action('dismiss');
      }
    };
    PageStack.prototype.ready = function(pageJSON) {
      if (!this.loadingPage) {
        throw new Error("Can't find loadingPage to be ready() on current page stack (" + this.rootContainerSelector + ").");
      }
      this.updateLegacyPage();
      this.loadingPage.ready(pageJSON);
      OF.GA.init();
      this.detach();
      this.activePage = this.loadingPage;
      this.loadingPage = null;
      return this.show();
    };
    PageStack.prototype.updateLegacyPage = function() {
      if (this === this.controller.activeStack) {
        OF.page = this.loadingPage || this.activePage;
        return console.log("Updating page: ", OF.page);
      } else {
        return console.log("This is not the stack you are looking for.");
      }
    };
    PageStack.prototype.loadPage = function(url, data, params, onComplete) {
      var page, _ref;
      if (this.init.isLoaded) {
        return;
      }
      if ((_ref = this.page) != null) {
        _ref.detach();
      }
      page = new Page(data, url, params);
      this.pages.push(page);
      this.loadTopPage(onComplete);
      return OF.GA.init();
    };
    PageStack.prototype.detach = function() {
      if (this.activePage != null) {
        this.activePage.deactivate();
      }
      return $(this.element).contents().detach();
    };
    PageStack.prototype.show = function() {
      if (this.activePage == null) {
        return;
      }
      $(this.element).append(this.activePage.nodes);
      this.activePage.activate();
      OF.init.isLoaded = true;
      if (window.runSpecs) {
        return OF.specs.run();
      }
    };
    PageStack.prototype.index = function(i) {
      return this.pages[i];
    };
    PageStack.prototype.literalReferenceString = function() {
      var i, index, _ref;
      index = 0;
      for (i = 0, _ref = OF.pageStackController.stacks.length; 0 <= _ref ? i <= _ref : i >= _ref; 0 <= _ref ? i++ : i--) {
        if (this === OF.pageStackController.stacks[i]) {
          index = i;
          break;
        }
      }
      return "OF.pageStackController.getStack('" + index + "')";
    };
    PageStack.prototype.contains = function(page) {
      return this.pages.indexOf(page) > -1;
    };
    PageStack.prototype.topPage = function() {
      return this.pages[this.pages.length - 1];
    };
    PageStack.prototype.activate = function() {
      return this.controller.activate(this);
    };
    return PageStack;
  })();
  Page = (function() {
    Page.State = {
      initialising: {
        toString: "Page is initialising, and has not yet requested its data"
      },
      awaitingJSON: {
        toString: "Page is waiting for the JSON from the native code / ajax request."
      },
      loaded: {
        toString: "The page has received the JSON data and is finalising its setup."
      },
      active: {
        toString: "Page is currently active (visible.)"
      },
      popped: {
        toString: "Page is not active, and has had its contents popped off of the document."
      }
    };
    function Page(url, options, stack) {
      var normalized_path;
      this.url = url;
      this.stack = stack;
      this.state = Page.State.initialising;
      this.onComplete = options.complete;
      this.setParams(options.params || {});
      this.flow = (/^(.*)\//.exec(this.url))[1];
      normalized_path = this.url.replace(/^(\/)/, '').replace(/\W+/g, '-');
      this.id = "page_" + normalized_path + "_" + (new Date().getTime());
      this.scrollPosition = 0;
      this.savedFunctions = {};
      this.node = null;
    }
    Page.prototype.startLoading = function() {
      this.state = Page.State.awaitingJSON;
      if (OF.isBrowser) {
        return this.ajaxLoad();
      } else {
        OF.log('sending startloading action');
        return OF.action.now('startLoading', {
          path: this.url,
          stackId: this.stack.stackId
        });
      }
    };
    Page.prototype.ajaxLoad = function() {
      return $.ajax({
        url: this.url,
        dataType: 'json',
        success: __bind(function(pageJSON) {
          return OF.push.ready(pageJSON);
        }, this),
        error: __bind(function(xhr) {
          return this.loadFailed(xhr);
        }, this)
      });
    };
    Page.prototype.loadFailed = function(xhr) {
      OF.alert("Error", "Screen loading failed:\n" + xhr.status + " " + xhr.statusText);
      return OF.loader.hide();
    };
    Page.prototype.ready = function(pageJSON) {
      this.loadData(pageJSON);
      this.loadHtml();
      this.loadScripts();
      this.state = Page.State.loaded;
      return document.title = this.title;
    };
    Page.prototype.loadData = function(pageData) {
      return U.extend(this, pageData);
    };
    Page.prototype.loadHtml = function() {
      var pageRoot;
      this.eventContext = $('<div class="event_context"></div>');
      this.eventContext.html(this.html);
      this.eventHandleElement = $('<div class="eventHandle"></div>');
      this.eventContext.append(this.eventHandleElement);
      this.nodes = this.eventContext;
      this.sub$ = jQuery.sub();
      pageRoot = this.eventContext;
      return this.sub$.fn.init = function(selector, context, root) {
        if (context == null) {
          context = pageRoot;
        }
        return new jQuery.fn.init(selector, context, root);
      };
    };
    Page.prototype.eventHandle = function() {
      return this.eventHandleElement;
    };
    Page.prototype.loadScripts = function() {
      var functionBlock, _i, _len, _ref, _results;
      _ref = ['init', 'appear', 'resume', 'loadflow'];
      _results = [];
      for (_i = 0, _len = _ref.length; _i < _len; _i++) {
        functionBlock = _ref[_i];
        _results.push(this[functionBlock] != null ? this[functionBlock] = $.functionize(this[functionBlock], this.url, functionBlock) : void 0);
      }
      return _results;
    };
    Page.prototype.activate = function() {
      if (!this.stack.controller.isFlowLoaded(this.flow)) {
        if (this.loadflow) {
          this.loadflow(this.sub$);
        }
        this.stack.controller.markFlowAsLoaded(this.flow);
      }
      if (this.state === Page.State.loaded) {
        if (this.init) {
          this.init(this.sub$);
        }
      }
      if (this.appear) {
        this.appear(this.sub$);
      }
      if (this.state === Page.State.popped) {
        if (this.resume) {
          this.resume(this.sub$);
        }
      }
      this.state = Page.State.active;
      return this.contentLoaded({
        pageStackSize: this.stack.pages.length,
        stackId: this.stack.stackId
      });
    };
    Page.prototype.deactivate = function() {
      return this.state = Page.State.popped;
    };
    Page.prototype.push = function(url, options) {
      return this.stack.push(url, options);
    };
    Page.prototype.replace = function(url, options) {
      return this.stack.replace(url, options);
    };
    Page.prototype.refresh = function() {
      this.stack.detach();
      this.state = Page.State.loaded;
      this.loadHtml();
      return this.stack.show();
    };
    Page.prototype.isCurrent = function() {
      return this.stack.topPage() === this;
    };
    Page.prototype.contentLoaded = function(options) {
      var _ref;
      if (options == null) {
        options = {};
      }
            if ((_ref = options.title) != null) {
        _ref;
      } else {
        options.title = document.title || this.title;
      };
      if (this.titleImage) {
        options.titleImage = this.titleImage;
      }
      if (this.barButton) {
        options.barButton = this.barButton;
      }
      if (this.barButtonImage) {
        options.barButtonImage = this.barButtonImage;
      }
      OF.loader.hide();
      return OF.action.now('contentLoaded', options);
    };
    Page.prototype.barButton = function() {
      var buttonName, options, page;
      options = {};
      page = this.stack.page;
      buttonName = page.barButton || page.globalBarButton;
      if (page.barButton) {
        options.barButton = buttonName;
      }
      if (page.barButtonImage) {
        options.barButtonImage = page.barButtonImage;
      }
      return OF.action('addBarButton', options);
    };
    Page.prototype.pageViewTracking = function() {
      if (this.stack.topPage()) {
        return OF.GA.page("/webui/" + (this.stack.topPage().url));
      }
    };
    Page.prototype.params = function() {
      var hasQuery, page, query, _ref;
      page = this.stack.topPage();
            if ((_ref = page.params) != null) {
        _ref;
      } else {
        page.params = {};
      };
      hasQuery = page.url.match(/\?/);
      if (hasQuery) {
        query = page.url.split('?')[1];
        return $.extend(page.params, $.urlDecode(query));
      }
    };
    Page.prototype.saveFunction = function(fn) {
      var pageIndex, string;
      if ($.isFunction(fn)) {
        string = U.uniqueId('fn');
        this.savedFunctions[string] = fn;
        pageIndex = this.stack.pages.indexOf(this);
        return "" + (this.stack.literalReferenceString()) + ".index(" + pageIndex + ").savedFunctions." + string;
      }
    };
    Page.prototype.exists = function() {
      return this.stack.contains(this);
    };
    Page.prototype.setParams = function(params) {
      var hasQuery, query;
      this.params = U.extend({}, params);
      hasQuery = this.url.match(/\?/);
      if (hasQuery) {
        query = this.url.split('?')[1];
        return $.extend(this.params, $.urlDecode(query));
      }
    };
    Page.prototype.timeout = function(delay, fn) {
      var self;
      self = this;
      return setTimeout(function() {
        if (self.exists()) {
          return fn();
        }
      }, delay);
    };
    Page.prototype.isCurrent = function() {
      return this.stack.topPage() === this;
    };
    Page.prototype.detach = function() {
      return this.nodes = this.stack.detach();
    };
    Page.prototype.attach = function() {
      return this.stack.attach(this.nodes);
    };
    return Page;
  })();
  OF.pageStackController = new PageStackController();
  OF.pageStackController.registerAsGlobal('OF.pageStackController');
  OF.pageStackController.createDefaultStack();
  $(function() {
    return OF.pageStackController.browser();
  });
  OF.push = function() {
    return OF.pageStackController.legacyPush.apply(OF.pageStackController, arguments);
  };
  OF.push.ready = function() {
    return OF.pageStackController.legacyReady.apply(OF.pageStackController, arguments);
  };
  OF.goBack = function() {
    return OF.pageStackController.legacyGoBack.apply(OF.pageStackController, arguments);
  };
  OF.pages.replace = function() {
    return OF.pageStackController.legacyReplace.apply(OF.pageStackController, arguments);
  };
  OF.refresh = function() {
    return OF.pageStackController.legacyRefresh.apply(OF.pageStackController, arguments);
  };
  OF.navigationStack = OF.pages;
  OF.navigateToUrlCallback = OF.push.ready;
  OF.topNavigationItem = $.deprecate(OF.topPage, 'OF.topNavigationItem()', 'OF.topPage()');
  OF.navigateToUrl = $.deprecate(OF.push, 'OF.navigateToUrl()', 'OF.push()');
}).call(this);
