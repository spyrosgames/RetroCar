// DO NOT EDIT
//   Generated from javascripts/dashboard/games.coffee
//
(function() {
  var __bind = function(fn, me){ return function(){ return fn.apply(me, arguments); }; };
  this.GamesPage = (function() {
    GamesPage.iDontHaverGamePerPage = 20;
    GamesPage.commonGamePerPage = 20;
    function GamesPage() {
      this.initUserInfo();
      this.gameList = {};
      this.iDonotHaveGameListPage = 1;
      this.loadIDonotHaveGameList();
      this.commonGameListPage = 1;
      this.loadCommonGameList();
      this.bindBehaviors();
    }
    GamesPage.prototype.initUserInfo = function() {
      this.userId = OF.page.params.user_id;
      this.localUserId = LocalUser.getLocalUser().id;
      return this.userName = OF.page.params.user_name;
    };
    GamesPage.prototype.iDonotHaveGameListUrl = function() {
      return "/xp/users/" + this.localUserId + "," + this.userId + "/games/i_do_not_have";
    };
    GamesPage.prototype.loadIDonotHaveGameList = function() {
      return OF.api(this.iDonotHaveGameListUrl(), {
        loader: "#others_loader",
        params: {
          page: this.iDonotHaveGameListPage
        },
        success: __bind(function(data) {
          this.renderIDonotHaveGameList(data.games);
          return this.saveGameList(data.games);
        }, this),
        failure: __bind(function() {
          OF.alert('Oops', 'Oops! An error has occured.');
          this.appendNodesToOthersGameList('');
          return $('#others_load_more').show();
        }, this)
      });
    };
    GamesPage.prototype.toggleIDonotHaveGameContainer = function(gamesLength) {
      var othersGameContainer;
      if (this.iDonotHaveGameListPage === 1) {
        othersGameContainer = $('#others_games');
        if (gamesLength > 0) {
          return othersGameContainer.unhide();
        } else {
          return othersGameContainer.addClass('hidden');
        }
      }
    };
    GamesPage.prototype.toggleIDonotHaveGameLoadMoreButton = function(gamesLength) {
      var loadMoreButton;
      loadMoreButton = $('#others_load_more');
      if (gamesLength > this.constructor.iDontHaverGamePerPage) {
        return loadMoreButton.unhide();
      } else {
        return loadMoreButton.addClass('hidden');
      }
    };
    GamesPage.prototype.appendNodesToOthersGameList = function(gameListHtml) {
      var loadMoreButton, loaderNode;
      loaderNode = $('#others_loader');
      loadMoreButton = $('#others_load_more');
      return $('#others_game_list').append(gameListHtml, loaderNode, loadMoreButton);
    };
    GamesPage.prototype.renderIDonotHaveGameList = function(games) {
      var gamesLength;
      gamesLength = games.length;
      this.toggleIDonotHaveGameContainer(gamesLength);
      if (gamesLength > 0) {
        this.appendNodesToOthersGameList(OtherGame.renderOtherGames(games));
      }
      return this.toggleIDonotHaveGameLoadMoreButton(gamesLength);
    };
    GamesPage.prototype.commonGameListUrl = function() {
      return "/xp/users/" + this.localUserId + "," + this.userId + "/games/common";
    };
    GamesPage.prototype.loadCommonGameList = function() {
      return OF.api(this.commonGameListUrl(), {
        loader: "#shared_loader",
        params: {
          page: this.commonGameListPage
        },
        success: __bind(function(data) {
          this.renderCommonGameList(data.games);
          return this.saveGameList(data.games);
        }, this),
        failure: function() {
          OF.alert('Oops', 'Oops! An error has occured.');
          appendNodesToCommonGameList('');
          return $('#shared_load_more').show();
        }
      });
    };
    GamesPage.prototype.toggleCommonGameContainer = function(gamesLength) {
      var commonGameContainer;
      if (this.commonGameListPage === 1) {
        commonGameContainer = $('#shared_games');
        if (gamesLength > 0) {
          return commonGameContainer.unhide();
        } else {
          return commonGameContainer.addClass('hidden');
        }
      }
    };
    GamesPage.prototype.toggleCommonLoadMoreButton = function(gamesLength) {
      var loadMoreButton;
      loadMoreButton = $('#shared_load_more');
      if (gamesLength > this.constructor.commonGamePerPage) {
        return loadMoreButton.unhide();
      } else {
        return loadMoreButton.addClass('hidden');
      }
    };
    GamesPage.prototype.appendNodesToCommonGameList = function(gameListHtml) {
      var loadMoreButton, loaderNode;
      loaderNode = $('#shared_loader');
      loadMoreButton = $('#shared_load_more');
      return $('#shared_game_list').append(gameListHtml, loaderNode, loadMoreButton);
    };
    GamesPage.prototype.renderCommonGameList = function(games) {
      var gamesLength;
      gamesLength = games.length;
      this.toggleCommonGameContainer(gamesLength);
      if (gamesLength > 0) {
        this.appendNodesToCommonGameList(CommonGame.renderCommonGames(games));
      }
      return this.toggleCommonLoadMoreButton(gamesLength);
    };
    GamesPage.prototype.saveGameList = function(games) {
      var gameObj, _i, _len, _results;
      _results = [];
      for (_i = 0, _len = games.length; _i < _len; _i++) {
        gameObj = games[_i];
        _results.push(this.gameList[String(gameObj.game.id)] = gameObj.game);
      }
      return _results;
    };
    GamesPage.prototype.bindBehaviors = function() {
      $(".game_cell." + OF.platform + "_platform").touch(__bind(function(event) {
        return this.gameCellListener(event);
      }, this));
      $(".get_game." + OF.platform + "_platform").touch(__bind(function(event) {
        return this.getGameListener(event);
      }, this));
      $('#others_load_more').touch(__bind(function(event) {
        return this.loadMoreIDonotHaveGame(event);
      }, this));
      return $('#shared_load_more').touch(__bind(function(event) {
        return this.loadMoreCommonGame(event);
      }, this));
    };
    GamesPage.prototype.gameUrl = function() {
      return 'dashboard/game';
    };
    GamesPage.prototype.gameCellListener = function(event) {
      var currentNode, gameId, gameObj;
      currentNode = $(event.target);
      gameId = currentNode.data('id');
      gameObj = this.gameList[gameId];
      return OF.push(this.gameUrl(), {
        params: {
          game: gameObj
        }
      });
    };
    GamesPage.prototype.getGameLog = function(packagename) {
      return "/webui/dashboard/games_openMarket?packagename=" + packagename;
    };
    GamesPage.prototype.getGameListener = function(event) {
      var node, packageName;
      node = $(event.target);
      packageName = node.data('packagename');
      OF.GA.page(this.getGameLog(packageName));
      return OF.action('openMarket', {
        package_name: packageName
      });
    };
    GamesPage.prototype.loadMoreIDonotHaveGame = function(event) {
      this.iDonotHaveGameListPage += 1;
      $(event.target).hide();
      return this.loadIDonotHaveGameList();
    };
    GamesPage.prototype.loadMoreCommonGame = function(event) {
      this.commonGameListPage += 1;
      $(event.target).hide();
      return this.loadCommonGameList();
    };
    return GamesPage;
  })();
}).call(this);
