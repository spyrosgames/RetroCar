// DO NOT EDIT
//   Generated from javascripts/dashboard/user.coffee
//
(function() {
  var __bind = function(fn, me){ return function(){ return fn.apply(me, arguments); }; };
  this.UserPage = (function() {
    var portraitPlayersThreshold;
    portraitPlayersThreshold = 4;
    function UserPage() {
      this.replaceHeader();
      this.bindLocalUserLoaded();
      if (!this.isSpotlight()) {
        this.loadGameInfo();
        this.loadAchievementsInfo();
      }
      this.bindBehaviors();
    }
    UserPage.prototype.replaceHeader = function() {
      if ($('#header_replacement').length > 0) {
        $('#header').remove();
        return $('#header_replacement').attr('id', 'header');
      }
    };
    UserPage.prototype.updateGameName = function() {
      return $('.game_name').html(OF.game.name);
    };
    UserPage.prototype.bindLocalUserLoaded = function() {
      return OF.localUserLoaded(__bind(function() {
        $('#info_cell .loading').hide();
        return this.renderUserInfo(OF.user);
      }, this));
    };
    UserPage.prototype.renderUserInfo = function(user) {
      LocalUser.updateLocalUser(user);
      this.pageUpdate();
      if (LocalUser.isFeatureEnabled('developer_announcement') && !this.isSpotlight()) {
        this.loadDeveloperAnnouncement();
      }
      if (LocalUser.isFeatureEnabled('who_is_playing') && !this.isSpotlight()) {
        return this.loadPlayersInfo();
      }
    };
    UserPage.prototype.pageUpdate = function() {
      $('#local_user_info').html(LocalUser.renderLocalUser('locaLUser'));
      this.updateGameName();
      this.toggleFriendsNotification();
      this.toggleWallNotificatioin();
      return this.toggleActionBanner();
    };
    UserPage.prototype.toggleFriendsNotification = function() {
      var friendsNotification;
      friendsNotification = $('#friends_notification');
      if (LocalUser.getLocalUser().friendNum() > 0) {
        return friendsNotification.unhide();
      } else {
        return friendsNotification.addClass('hidden');
      }
    };
    UserPage.prototype.toggleWallNotificatioin = function() {
      var wallNotification;
      wallNotification = $('#wall_notification');
      if (LocalUser.getLocalUser().wallNum() > 0) {
        return wallNotification.unhide();
      } else {
        return wallNotification.addClass('hidden');
      }
    };
    UserPage.prototype.findFriendsUrl = function() {
      return 'dashboard/me/friends?find_friends=true';
    };
    UserPage.prototype.completeProfileUrl = function() {
      return 'settings/profile_configuration';
    };
    UserPage.prototype.toggleActionBanner = function() {
      var actionBanner, completeProfile, findFriends;
      actionBanner = $('#action_banner');
      completeProfile = actionBanner.find('.complete_profile');
      findFriends = actionBanner.find('.find_friends');
      if (LocalUser.getLocalUser().isCompletedProfile()) {
        completeProfile.addClass('hidden');
        return findFriends.unhide();
      } else {
        completeProfile.unhide();
        return findFriends.addClass('hidden');
      }
    };
    UserPage.prototype.isSpotlight = function() {
      return OF.page.params.spotlight != null;
    };
    UserPage.prototype.developerAnnouncementUrl = function() {
      return "/xp/games/" + OF.game.id + "/announcements";
    };
    UserPage.prototype.loadDeveloperAnnouncement = function() {
      return OF.api(this.developerAnnouncementUrl(), {
        background: true,
        success: __bind(function(data) {
          return this.renderDeveloperAnnouncement(data.announcements);
        }, this)
      });
    };
    UserPage.prototype.renderDeveloperAnnouncement = function(announcements) {
      if (announcements.length > 0) {
        $('#announcement').html(DeveloperAnnouncement.renderDashboardAnnouncement(announcements[0].announcement));
        return $('#developer_announcement').unhide();
      }
    };
    UserPage.prototype.gameInfoApiUrl = function() {
      return "/xp/games/" + OF.game.id;
    };
    UserPage.prototype.loadGameInfo = function() {
      return OF.api(this.gameInfoApiUrl(), {
        background: true,
        success: __bind(function(data) {
          return this.renderGameInfo(data.game);
        }, this)
      });
    };
    UserPage.prototype.renderGameInfo = function(game) {
      if (game.leaderboards.total > 0) {
        $('#leaderboards').unhide();
      }
      if (game.achievements.total > 0) {
        $('#achievements').unhide();
      }
      if (!game.approved_for_current_platform) {
        return $('#unapproved').unhide();
      }
    };
    UserPage.prototype.achievementsInfoApiUrl = function() {
      return "/xp/users/" + OF.user.id + "/games/" + OF.game.id + "/achievements";
    };
    UserPage.prototype.loadAchievementsInfo = function() {
      return OF.api(this.achievementsInfoApiUrl(), {
        background: true,
        failure: $.noop,
        success: __bind(function(data) {
          return this.renderAchievements(data.achievements);
        }, this)
      });
    };
    UserPage.prototype.renderAchievements = function(achievements) {
      var achievementObj, total, unlocked, _i, _len;
      total = achievements.length;
      unlocked = 0;
      for (_i = 0, _len = achievements.length; _i < _len; _i++) {
        achievementObj = achievements[_i];
        if (achievementObj.achievement.is_unlocked) {
          unlocked++;
        }
      }
      return $('#achievements').find('.subtitle').text("" + unlocked + " out of " + total + " unlocked").unhide();
    };
    UserPage.prototype.playersInfoApiUrl = function() {
      return "/xp/games/" + OF.game.id + "/users";
    };
    UserPage.prototype.loadPlayersInfo = function() {
      return OF.api(this.playersInfoApiUrl(), {
        background: true,
        success: __bind(function(data) {
          return this.renderPlayers(data.users.items);
        }, this)
      });
    };
    UserPage.prototype.renderPlayers = function(players) {
      var playersNode;
      this.toggleWhoIsPlayingContainer(players);
      if (players.length > 0) {
        playersNode = $('#players');
        return playersNode.html(Player.renderPlayers(players, 'playerTmplOfUser', portraitPlayersThreshold));
      }
    };
    UserPage.prototype.toggleWhoIsPlayingContainer = function(players) {
      var whoElsePlayingNode;
      whoElsePlayingNode = $('#who_else_playing');
      if (players.length > 0) {
        return whoElsePlayingNode.unhide();
      } else {
        return whoElsePlayingNode.addClass('hidden');
      }
    };
    UserPage.prototype.bindBehaviors = function() {
      $('#user_info').touch(__bind(function() {
        return this.infoCellListener();
      }, this));
      $('#friends_notification').touch(__bind(function() {
        return this.friendNotificationListener();
      }, this));
      $('#wall_notification').touch(__bind(function() {
        return this.wallNotificationListener();
      }, this));
      $('#action_banner').touch(__bind(function() {
        return this.actionBannerListener();
      }, this));
      return $('#achievements').touch(__bind(function() {
        return this.achievementsListener();
      }, this));
    };
    UserPage.prototype.infoCellUrl = function() {
      return 'dashboard/profile';
    };
    UserPage.prototype.infoCellListener = function() {
      return OF.push(this.infoCellUrl(), {
        params: {
          user: OF.user
        }
      });
    };
    UserPage.prototype.friendRequestUrl = function() {
      return 'dashboard/friend_requests';
    };
    UserPage.prototype.friendNotificationListener = function() {
      return OF.push(this.friendRequestUrl());
    };
    UserPage.prototype.wallNotificationUrl = function() {
      return 'dashboard/profile';
    };
    UserPage.prototype.wallNotificationListener = function() {
      return OF.push(this.wallNotificationUrl(), {
        params: {
          user: OF.user
        }
      });
    };
    UserPage.prototype.actionUrl = function() {
      if (LocalUser.getLocalUser().isCompletedProfile()) {
        return this.findFriendsUrl();
      } else {
        return this.completeProfileUrl();
      }
    };
    UserPage.prototype.actionBannerListener = function() {
      return setTimeout(__bind(function() {
        return OF.push(this.actionUrl());
      }, this), 300);
    };
    UserPage.prototype.achievementsUrl = function() {
      return 'dashboard/achievements';
    };
    UserPage.prototype.achievementsListener = function() {
      return OF.push(this.achievementsUrl(), {
        params: {
          user: OF.user
        }
      });
    };
    return UserPage;
  })();
}).call(this);
