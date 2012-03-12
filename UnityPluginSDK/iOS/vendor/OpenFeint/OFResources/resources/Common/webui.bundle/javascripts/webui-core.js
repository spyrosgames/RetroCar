// DO NOT EDIT
//   Generated from javascripts/webui-core.coffee
//
(function() {
  var OF, _ref, _ref2;
  var __slice = Array.prototype.slice;
  if (this.runSpecs == null) {
    this.runSpecs = false;
  }
  this.OF = OF = {};
  OF.isDevice = navigator.userAgent.match(/iPhone|iPad|Android/);
  OF.isBrowser = !OF.isDevice;
  if ((_ref = window.location) != null ? (_ref2 = _ref.href) != null ? _ref2.match(/isBrowser=[^&$]/) : void 0 : void 0) {
    OF.isDevice = false;
    OF.isBrowser = true;
  }
  ({
    deviceType: {
      iOS: navigator.userAgent.match(/iPhone|iPad/),
      Android: navigator.userAgent.match(/Android/)
    }
  });
  OF.hasNativeInterface = false;
  OF.page = null;
  OF.global = {};
  OF.orientation = null;
  OF.pages = [];
  OF.pages.replace = function(path) {
    OF.pages.splice(0, OF.pages.length);
    return OF.push(path);
  };
  OF.log = function(data) {
    var message;
    if (OF.isBrowser) {
      console.log('WEBLOG:', data);
    }
    if (OF.device.ios3) {
      return;
    }
    if (OF.isDevice) {
      if (typeof data === 'object') {
        message = $.urlEncode(data);
      } else {
        message = "" + data;
      }
      return OF.action('log', {
        message: message
      });
    }
  };
  OF.time = function() {
    var checkPointNames, now;
    checkPointNames = 1 <= arguments.length ? __slice.call(arguments, 0) : [];
    if (OF.device.ios3) {
      return;
    }
    now = Date.now();
    if (OF.isBrowser) {
      console.log("PROFILE " + now + " " + (checkPointNames.join('::')));
    }
    if (OF.isDevice) {
      return OF.action('profile', {
        time: now,
        checkPointNames: checkPointNames.join('::')
      });
    }
  };
  OF.setOrientation = function(newOrientation) {
    var _ref3;
    if (newOrientation) {
      OF.orientation = newOrientation;
      $('body').removeClass('orientation_portrait').removeClass('orientation_landscape').addClass("orientation_" + OF.orientation);
      return (_ref3 = OF.topPage()) != null ? typeof _ref3.orientationChanged === "function" ? _ref3.orientationChanged(OF.orientation) : void 0 : void 0;
    }
  };
  OF.DEBUG = {
    enableWeinre: function(host) {
      var e;
      e = document.createElement("script");
      e.setAttribute("src", "http://" + host + "/target/target-script-min.js#anonymous");
      return document.getElementsByTagName("body")[0].appendChild(e);
    }
  };
  OF.init = {
    isLoaded: false,
    flowIsLoaded: false,
    firstPage: function() {
      var options;
      if (OF.isBrowser) {
        options = $.urlDecode(location.href.split('?')[1]);
        $('html, body').css('-webkit-user-select', 'auto');
        if (options.url) {
          $.ajax({
            url: '/webui/browser_config.json',
            dataType: 'json',
            complete: function(xhr) {
              var browserConfig;
              browserConfig = JSON.parse(xhr.responseText);
              if (browserConfig.remoteDebugHost) {
                OF.DEBUG.enableWeinre(browserConfig.remoteDebugHost);
              }
              OF.init.clientBoot(browserConfig);
              return OF.push(options.url);
            }
          });
        } else {
          OF.alert('ERROR', 'No Content to Load! This page must be called with a url like /webui/index.html?url=some/content_path');
        }
      }
      OF.touch.bindHandlers();
      if ('onerror' in window) {
        $(window).error(function(e) {
          OF.Log("" + e.fielname + ":" + e.lineno + ":" + e.message);
          return false;
        });
      }
      return $(window).scroll(function(ev) {
        var time, _ref3;
        if (ev.target !== ev.currentTarget) {
          return;
        }
        time = new Date().getTime();
        if (window._lastScrollingTime && (time - window._lastScrollingTime) < 249) {
          return false;
        }
        window._lastScrollingTime = time;
        OF.touch.isScrolling = true;
        clearTimeout(window._stopScrollingCallback);
        if ((_ref3 = OF.page.eventHandle()) != null) {
          _ref3.trigger('scroll');
        }
        return window._stopScrollingCallback = setTimeout(function() {
          return OF.touch.isScrolling = false;
        }, 250);
      });
    },
    clientBoot: function(options) {
      var body;
      OF.hasNativeInterface = options.hasNativeInterface;
      OF.user = options.user;
      if (!OF.user.name) {
        OF.user.name = null;
      }
      if (!OF.user.id || OF.user.id.toString() === '0') {
        OF.user.id = null;
      }
      OF.game = options.game;
      OF.serverUrl = options.serverUrl;
      OF.actions = options.actions;
      OF.settings.enabled = OF.action.isSupported('readSetting') && OF.action.isSupported('writeSetting');
      OF.clientVersion = options.clientVersion;
      OF.platform = options.platform;
      OF.device = options.device;
      OF.device.ios3 = !!OF.device.os.match(/iPhone.*3\.\d\.\d/);
      if (OF.device.ios3) {
        OF.action.delay = 250;
      }
      OF.dpi = options.dpi;
      OF.setOrientation(options.orientation);
      OF.disableGA = options.disableGA;
      OF.supports = options.supports || {};
      OF.supports.fixedPosition = OF.platform === 'android' && OF.device.os.match(/v2\.2/) || OF.device.hardware === 'browser';
      OF.supports.nativeScrolling = 'webkitOverflowScrolling' in document.documentElement.style;
      OF.action.sendController.init();
      OF.manifestUrl = options.manifestUrl;
      OF.log("Client Booted - userID: " + OF.user.id + " gameID: " + OF.game.id + " platform: " + OF.platform + " dpi: " + OF.dpi);
      body = $('body');
      body.addClass(OF.dpi).addClass(OF.platform);
      if (OF.supports.fixedPosition) {
        body.addClass('fixed_position');
      }
      return true;
    },
    start: function() {
      var buttonTitle, options, pageRoot, sub$, _base, _ref3;
      OF.init.isLoaded = false;
      OF.init.scripts();
      OF.init.browser();
      OF.init.params();
      if (!OF.init.flowIsLoaded && OF.page.loadflow) {
        OF.page.loadflow();
        OF.init.flowIsLoaded = true;
      }
      sub$ = jQuery.sub();
      pageRoot = OF.page.eventContext;
      sub$.fn.init = function(selector, context, root) {
        if (context == null) {
          context = pageRoot;
        }
        return new jQuery.fn.init(selector, context, root);
      };
            if ((_ref3 = (_base = OF.page).init) != null) {
        _ref3;
      } else {
        _base.init = $.noop;
      };
      if (OF.page.init.complete) {
        if (OF.page.resume) {
          U.defer(function() {
            try {
              return OF.page.resume(sub$);
            } catch (e) {
              return OF.alert('ERROR', "A script on this screen caused an error.\n resume: " + (e.toString()));
            }
          });
        }
      } else {
        U.defer(function() {
          try {
            OF.page.init(sub$);
          } catch (e) {
            OF.alert('ERROR', "A script on this screen caused an error.\n init: " + (e.toString()));
          }
          return OF.page.init.complete = true;
        });
      }
      U.defer(function() {
        if (OF.page.appear) {
          try {
            return OF.page.appear(sub$);
          } catch (e) {
            return OF.alert('ERROR', "A script on this screen caused an error.\n appear: " + (e.toString()));
          }
        }
      });
      U.defer(OF.init.pageViewTracking);
      OF.init.isLoaded = true;
      buttonTitle = OF.page.barButton || OF.page.globalBarButton;
      options = {};
      if (buttonTitle) {
        options.barButton = buttonTitle;
      }
      return U.defer(function() {
        if (!OF.device.ios3 || (OF.device.ios3 && OF.api.activeRequestIDs.length === 0)) {
          document.title = OF.page.title;
          console.error('CL', options);
          return OF.contentLoaded(options);
        }
      });
    },
    scripts: function() {
      if ((OF.page.init != null) && !$.isFunction(OF.page.init)) {
        OF.page.init = $.functionize(OF.page.init, OF.page.url, 'init');
        OF.page.init.complete = false;
      }
      if ((OF.page.appear != null) && !$.isFunction(OF.page.appear)) {
        OF.page.appear = $.functionize(OF.page.appear, OF.page.url, 'appear');
      }
      if ((OF.page.resume != null) && !$.isFunction(OF.page.resume)) {
        OF.page.resume = $.functionize(OF.page.resume, OF.page.url, 'resume');
      }
      if (!OF.init.flowIsLoaded && (OF.page.loadflow != null) && !$.isFunction(OF.page.loadflow)) {
        return OF.page.loadflow = $.functionize(OF.page.loadflow, OF.page.url, 'loadflow');
      }
    },
    browser: function() {
      if (OF.isBrowser && $('#browser_toolbar').length === 0) {
        $.loadCss('browser_toolbar', false);
        return $.get('browser_toolbar.html', function(data) {
          return $(document.body).append(data);
        });
      }
    },
    barButton: function() {
      var buttonName, options;
      options = {};
      buttonName = OF.page.barButton || OF.page.globalBarButton;
      if (OF.page.barButton) {
        options.barButton = buttonName;
      }
      if (OF.page.barButtonImage) {
        options.barButtonImage = OF.page.barButtonImage;
      }
      return OF.action('addBarButton', options);
    },
    pageViewTracking: function() {
      if (OF.topPage()) {
        return OF.GA.page("/webui/" + (OF.topPage().url));
      }
    },
    params: function() {
      var page, _ref3;
      page = OF.topPage();
            if ((_ref3 = page.params) != null) {
        _ref3;
      } else {
        page.params = {};
      };
      if (page.url.match(/\?/)) {
        return $.extend(page.params, $.urlDecode(page.url.split('?')[1]));
      }
    }
  };
  OF.forceSetTitle = function(title) {
    var titleElement;
    if ((titleElement = $('#header .title')).length > 0) {
      return titleElement.html(title);
    }
  };
  OF.topPage = function() {
    return OF.pages[OF.pages.length - 1];
  };
  OF.pushController = function(controllerName, options) {
    controllerName = "" + controllerName + "?" + ($.urlEncode(options));
    if (OF.isDevice) {
      location.href = "openfeint://controller/" + controllerName;
    }
    return OF.log("CONTROLLER: " + controllerName);
  };
  OF.barButton = function(title, onTouch) {
    var options;
    options = {};
    if (title.match(/\.png$/)) {
      options.image = title.replace('xdpi.png', "" + OF.dpi + ".png");
    } else {
      options.title = title;
    }
    OF.page.barButtonTouch = onTouch;
    return OF.action('addBarButton', options);
  };
  OF.alert = function(title, message, options) {
    if (options == null) {
      options = {};
    }
    options.title = title;
    options.message = message;
    OF.action('alert', options);
    if (OF.isBrowser) {
      return alert("" + options.title + "\n\n" + options.message);
    }
  };
  OF.confirm = function(title, message, positive, negative, positiveCallback, negativeCallback) {
    var confirmMessage;
    if (OF.isBrowser || !OF.action.isSupported('confirm')) {
      confirmMessage = title.length > 0 ? "" + title + "\n\n" + message : "" + message;
      if (confirm(confirmMessage)) {
        return positiveCallback();
      } else {
        return typeof negativeCallback === "function" ? negativeCallback() : void 0;
      }
    } else {
      return OF.action('confirm', {
        title: title,
        message: message,
        positive: positive,
        negative: negative,
        callback: function(result) {
          if (result) {
            return positiveCallback();
          } else {
            return typeof negativeCallback === "function" ? negativeCallback() : void 0;
          }
        }
      });
    }
  };
  OF.loader = {
    count: 0,
    show: function() {
      if (OF.device.ios3) {
        return;
      }
      $('#header .loading').show();
      return OF.loader.count += 1;
    },
    hide: function() {
      if (OF.device.ios3) {
        return;
      }
      OF.loader.count -= 1;
      if (OF.loader.count < 0) {
        OF.loader.count = 0;
      }
      if (OF.loader.count === 0) {
        return $('#header .loading').hide();
      }
    }
  };
  OF.userDidLogin = function(user) {
    var _base, _ref3;
    if ((user != null ? (_ref3 = user.id) != null ? _ref3.toString().length : void 0 : void 0) && user.id.toString() !== '0') {
      OF.user = user;
    } else {
      OF.user = {
        name: null,
        id: null
      };
    }
    return typeof (_base = OF.page).userDidLogin === "function" ? _base.userDidLogin(user) : void 0;
  };
  OF.settings = {
    enabled: false,
    expectJsonAsString: null,
    clear: function(key) {
      return OF.settings.write(key, null);
    },
    write: function(key, value) {
      OF.action('writeSetting', {
        key: key,
        value: JSON.stringify(value)
      });
      if (OF.isBrowser) {
        return OF.settings.browser.write(key, value);
      }
    },
    read: function(key, callback) {
      var clientVersion, origCallback;
      if (OF.settings.expectJsonAsString == null) {
        clientVersion = OF.clientVersion ? OF.clientVersion.split('.') : [0, 0, 0];
        OF.settings.expectJsonAsString = OF.platform === 'android' && (parseInt(clientVersion[0], 10) || 0) <= 1 && (parseInt(clientVersion[1], 10) || 0) <= 7 && (parseInt(clientVersion[2], 10) || 0) <= 5;
      }
      if (OF.settings.expectJsonAsString) {
        origCallback = callback;
        callback = function(jsonStringVal) {
          return origCallback(jsonStringVal ? JSON.parse(jsonStringVal) : null);
        };
      }
      OF.action('readSetting', {
        key: key,
        callback: callback
      });
      if (OF.isBrowser || !OF.settings.enabled) {
        return callback(OF.settings.browser.read(key));
      }
    },
    browser: (function() {
      var obj, save, settingsObj;
      if (OF.isDevice) {
        obj = {};
        obj.clearAll = obj.write = obj.read = $.noop;
        return obj;
      }
      settingsObj = null;
      U.each(document.cookie.split('; '), function(cookie) {
        if (cookie.split('=')[0] === 'WEBUI_SETTINGS') {
          try {
            return settingsObj = JSON.parse(decodeURIComponent(cookie.split('=')[1]));
          } catch (e) {
            return settingsObj = {};
          }
        }
      });
      settingsObj || (settingsObj = {});
      save = function() {
        return document.cookie = "WEBUI_SETTINGS=" + (encodeURIComponent(JSON.stringify(settingsObj)));
      };
      return {
        clearAll: function() {
          settingsObj = {};
          return save();
        },
        write: function(key, value) {
          settingsObj[key] = value;
          return save();
        },
        read: function(key) {
          var val;
          if ((val = settingsObj[key]) != null) {
            return val;
          } else {
            return null;
          }
        }
      };
    })()
  };
  OF.specs = {
    load: function(pagePath) {
      var flow;
      if (window.runSpecs) {
        OF.api.allow = false;
        flow = pagePath.split('/')[0];
        $.loadScript("../spec/" + flow + "/index");
        jasmine.WaitsForBlock.TIMEOUT_INCREMENT = OF.isDevice ? 500 : 100;
        if (OF.device.ios3) {
          jasmine.WaitsForBlock.TIMEOUT_INCREMENT = 1000;
          return jasmine.DEFAULT_TIMEOUT_INTERVAL = 10000;
        }
      }
    },
    run: function() {
      if (window.runSpecs && OF.pages.length === 1) {
        return window.runSpecs = false;
      }
    }
  };
  if (typeof NativeInterface !== "undefined" && NativeInterface !== null) {
    if (typeof NativeInterface.frameworkLoaded === "function") {
      NativeInterface.frameworkLoaded();
    }
  }
  $(document).ready(OF.init.firstPage);
}).call(this);
