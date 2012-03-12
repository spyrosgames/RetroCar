// DO NOT EDIT
//   Generated from javascripts/dashboard/players.coffee
//
(function() {
  var __bind = function(fn, me){ return function(){ return fn.apply(me, arguments); }; };
  this.PlayersPage = (function() {
    PlayersPage.playersPerPage = 7;
    function PlayersPage() {
      this.page = 1;
      this.load();
      this.bindBehaviors();
    }
    PlayersPage.prototype.apiUrl = function() {
      return "/xp/games/" + OF.game.id + "/users/more";
    };
    PlayersPage.prototype.load = function() {
      return OF.api(this.apiUrl(), {
        background: true,
        params: {
          page: this.page
        },
        success: __bind(function(data) {
          return this.render(data);
        }, this)
      });
    };
    PlayersPage.prototype.renderForFirstTime = function() {
      return this.page === 1;
    };
    PlayersPage.prototype.render = function(data) {
      if (this.renderForFirstTime()) {
        this.renderPlayers(data.users.friends, '#friends');
      }
      return this.renderPlayers(data.users.strangers, '#everyone');
    };
    PlayersPage.prototype.renderPlayers = function(players, containerSelector) {
      var $container, $loadMore, $playerList, hasNextPage;
      $container = $(containerSelector);
      $loadMore = $container.find('#load_more');
      if (players.length > 0) {
        $loadMore.detach();
        $playerList = $container.find(".player_list");
        $playerList.append(Player.renderPlayers(players, 'playerTmplOfPlayers'), $loadMore);
      }
      hasNextPage = players.length > this.constructor.playersPerPage;
      if (hasNextPage) {
        $loadMore.unhide();
      } else {
        $loadMore.addClass('hidden');
      }
      if (this.renderForFirstTime()) {
        return this.toggleContainer($container, players);
      }
    };
    PlayersPage.prototype.toggleContainer = function($container, players) {
      if (players.length > 0) {
        return $container.unhide();
      } else {
        return $container.addClass('hidden');
      }
    };
    PlayersPage.prototype.loadMore = function() {
      this.page += 1;
      return this.load();
    };
    PlayersPage.prototype.loadMoreListener = function() {
      OF.GA.page('/webui/dashboard/players_loadMorePlayers');
      return this.loadMore();
    };
    PlayersPage.prototype.bindBehaviors = function() {
      return $('#load_more').touch(__bind(function() {
        return this.loadMoreListener();
      }, this));
    };
    return PlayersPage;
  })();
}).call(this);
