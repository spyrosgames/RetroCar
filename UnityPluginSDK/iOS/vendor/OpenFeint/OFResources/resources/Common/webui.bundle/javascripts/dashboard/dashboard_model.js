// DO NOT EDIT
//   Generated from javascripts/dashboard/dashboard_model.coffee
//
(function() {
  var __indexOf = Array.prototype.indexOf || function(item) {
    for (var i = 0, l = this.length; i < l; i++) {
      if (this[i] === item) return i;
    }
    return -1;
  };
  this.UserHelper = (function() {
    function UserHelper() {}
    UserHelper.numberTrim = function(num) {
      if (num > 999) {
        return num = 999;
      } else {
        return num;
      }
    };
    return UserHelper;
  })();
  this.LocalUser = (function() {
    function LocalUser() {
      U.extend(this, OF.user);
    }
    LocalUser.renderLocalUser = function(templateName) {
      this.template = tmpl(templateName);
      return this.getLocalUser().html();
    };
    LocalUser.getLocalUser = function() {
      return new LocalUser;
    };
    LocalUser.updateLocalUser = function(user) {
      return OF.user = user;
    };
    LocalUser.isLocalUser = function(userId) {
      return parseInt(userId) === parseInt(OF.user.id);
    };
    LocalUser.isFeatureEnabled = function(featureName) {
      var userFeatures;
      userFeatures = LocalUser.getLocalUser().features;
      return (userFeatures != null) && __indexOf.call(userFeatures, featureName) >= 0;
    };
    LocalUser.prototype.isSetName = function() {
      return OF.user.has_set_name;
    };
    LocalUser.prototype.isSetProfilePicture = function() {
      return OF.user.profile_picture_url != null;
    };
    LocalUser.prototype.isCompletedProfile = function() {
      return this.isSetName() && this.isSetProfilePicture();
    };
    LocalUser.prototype.avatarStyle = function() {
      if (this.isSetProfilePicture()) {
        return "background-image: url(" + OF.user.profile_picture_url + ")";
      } else {
        return "";
      }
    };
    LocalUser.prototype.friendNum = function() {
      return UserHelper.numberTrim(OF.user.received_friend_requests_pending_count);
    };
    LocalUser.prototype.wallNum = function() {
      return UserHelper.numberTrim(OF.user.not_viewed_wall_items_count);
    };
    LocalUser.prototype.html = function() {
      return this.constructor.template(this);
    };
    return LocalUser;
  })();
  this.PageUser = (function() {
    function PageUser(pageObj) {
      pageObj.user = pageObj.params.user || {
        id: pageObj.params.user_id
      };
      U.extend(this, pageObj.user);
      this.isLocalUser = LocalUser.isLocalUser(this.id);
    }
    PageUser.prototype.updatePageUser = function(user) {
      U.extend(this, user);
      if (this.isLocalUser) {
        LocalUser.updateLocalUser(user);
        return LocalUser.getLocalUser().not_viewed_wall_items_count = 0;
      }
    };
    PageUser.prototype.style = function() {
      if (this.profile_picture_url != null) {
        return "background-image: url(" + this.profile_picture_url + ")";
      } else {
        return "";
      }
    };
    PageUser.prototype.userStatus = function() {
      if (this.isLocalUser && this.status.length === 0) {
        return 'What are you up to?';
      } else {
        return this.status;
      }
    };
    PageUser.prototype.statusClassName = function() {
      if (!this.isLocalUser && this.status.length === 0) {
        return 'hidden';
      } else {
        return '';
      }
    };
    PageUser.prototype.gameTitle = function() {
      var gameString;
      gameString = this.games_count <= 1 ? 'Game' : 'Games';
      return "" + this.games_count + " " + gameString;
    };
    PageUser.prototype.friendTitle = function() {
      var friendString;
      friendString = this.friendship_count <= 1 ? 'Friend' : 'Friends';
      return "" + this.friendship_count + " " + friendString;
    };
    PageUser.prototype.pendingFriend = function() {
      if (this.isLocalUser) {
        return UserHelper.numberTrim(this.received_friend_requests_pending_count);
      }
    };
    PageUser.prototype.renderBasicInfo = function() {
      this.basicTemplate = tmpl('pageUser_tmpl');
      return this.basicInfoHtml();
    };
    PageUser.prototype.basicInfoHtml = function() {
      return this.basicTemplate(this);
    };
    PageUser.prototype.renderGameFriend = function() {
      this.gameFriendTemplate = tmpl('gameFriend_tmpl');
      return this.gameFriendHtml();
    };
    PageUser.prototype.gameFriendHtml = function() {
      return this.gameFriendTemplate(this);
    };
    return PageUser;
  })();
  this.Player = (function() {
    function Player(player, playerIndex) {
      this.player = player;
      this.playerIndex = playerIndex;
      U.extend(this, this.player);
    }
    Player.prototype.style = function() {
      if (this.profile_picture_url != null) {
        return "background-image: url(" + this.profile_picture_url + ")";
      } else {
        return "";
      }
    };
    Player.prototype.cssClass = function() {
      var baseClass;
      baseClass = '';
      if ((this.constructor.portraitPlayersThreshold != null) && this.playerIndex > this.constructor.portraitPlayersThreshold - 1) {
        baseClass = 'additional';
      }
      if (this.online) {
        baseClass += ' online';
      } else {
        baseClass += ' offline';
      }
      if (this.friendship) {
        baseClass += ' friend';
      }
      return baseClass;
    };
    Player.prototype.profileUrl = function() {
      return "dashboard/profile?user_id=" + this.id;
    };
    Player.prototype.html = function() {
      return this.constructor.template(this);
    };
    Player.renderPlayers = function(players, templateName, portraitPlayersThreshold) {
      var index, p, player, playerHtml, _len;
      this.portraitPlayersThreshold = portraitPlayersThreshold;
      playerHtml = '';
      this.template = tmpl(templateName);
      for (index = 0, _len = players.length; index < _len; index++) {
        player = players[index];
        if (index >= 7) {
          break;
        }
        p = new Player(player, index);
        playerHtml += p.html();
      }
      return playerHtml;
    };
    return Player;
  })();
  this.Wall = (function() {
    function Wall(wall, index) {
      this.wall = wall;
      this.index = index;
      U.extend(this, this.wall);
    }
    Wall.prototype.style = function() {
      if (this.profile_picture_url != null) {
        return "background-image: url(" + this.profile_picture_url + ")";
      } else {
        return "";
      }
    };
    Wall.prototype.profileUrl = function() {
      return "dashboard/profile?user_id=" + this.user_id;
    };
    Wall.prototype.className = function() {
      var className;
      className = '';
      if (this.index < this.constructor.newItemNum) {
        className += ' new_message';
      }
      if (this.constructor.pageUser.isLocalUser || LocalUser.isLocalUser(this.user_id)) {
        className += ' showDeleteButton';
      }
      return className;
    };
    Wall.prototype.message = function() {
      if (this.type === 'wall_invitation') {
        return "Hey " + this.invite_name + ", I'm playinng <span class=\"invite_game\" data-packagename=\"" + this.package_identifier + "\">" + this.game_name + "</span> and want you to come play with me! Download the game and sign into OpenFeint to invite more friends";
      } else {
        return this.body;
      }
    };
    Wall.prototype.html = function() {
      return this.constructor.template(this);
    };
    Wall.renderWalls = function(messages, newItemNum, pageUser) {
      var index, m, message, wallHtml, _len;
      this.newItemNum = newItemNum;
      this.pageUser = pageUser;
      this.template = tmpl('message_tmpl');
      wallHtml = '';
      for (index = 0, _len = messages.length; index < _len; index++) {
        message = messages[index];
        if (index >= 10) {
          break;
        }
        m = new Wall(message, index);
        wallHtml += m.html();
      }
      return wallHtml;
    };
    return Wall;
  })();
  this.DeveloperAnnouncement = (function() {
    DeveloperAnnouncement.announcePerPage = 20;
    function DeveloperAnnouncement(announcement) {
      this.announcement = announcement;
      U.extend(this, this.announcement);
    }
    DeveloperAnnouncement.prototype.html = function() {
      return this.constructor.template(this);
    };
    DeveloperAnnouncement.renderDashboardAnnouncement = function(announcement) {
      var announce;
      this.template = tmpl('dashboardAnnounceTmpl');
      announce = new DeveloperAnnouncement(announcement);
      return announce.html();
    };
    DeveloperAnnouncement.renderSingleAnnouncement = function(announcement) {
      var announce;
      this.template = tmpl('singleAnnounceTmpl');
      announce = new DeveloperAnnouncement(announcement);
      return announce.html();
    };
    DeveloperAnnouncement.renderAnnouncementList = function(announcements) {
      var ann, announce, announcementHtml, index, _len;
      this.template = tmpl('announceTmpl');
      announcementHtml = '';
      for (index = 0, _len = announcements.length; index < _len; index++) {
        announce = announcements[index];
        if (index === this.constructor.announcePerPage) {
          break;
        }
        ann = new DeveloperAnnouncement(announce.announcement);
        announcementHtml += ann.html();
      }
      return announcementHtml;
    };
    return DeveloperAnnouncement;
  })();
  this.TopGame = (function() {
    TopGame.portraitTopGameThreshold = 4;
    function TopGame(game, topGameIndex) {
      this.game = game;
      this.topGameIndex = topGameIndex;
      U.extend(this, this.game);
    }
    TopGame.prototype.style = function() {
      if (this.game_icon_url != null) {
        return "background-image: url(" + this.game_icon_url + ")";
      } else {
        return "";
      }
    };
    TopGame.prototype.cssClass = function() {
      var baseClass;
      baseClass = '';
      if ((this.constructor.portraitTopGameThreshold != null) && this.topGameIndex > this.constructor.portraitTopGameThreshold - 1) {
        return baseClass = 'additional';
      }
    };
    TopGame.prototype.html = function() {
      return this.constructor.template(this);
    };
    TopGame.renderTopGames = function(topGames) {
      var g, index, topGame, topGameHtml, _len;
      topGameHtml = '';
      this.template = tmpl('topGameTml');
      for (index = 0, _len = topGames.length; index < _len; index++) {
        topGame = topGames[index];
        g = new TopGame(topGame, index);
        topGameHtml += g.html();
      }
      return topGameHtml;
    };
    return TopGame;
  })();
  this.OtherGame = (function() {
    function OtherGame(game) {
      this.game = game;
      U.extend(this, this.game);
    }
    OtherGame.prototype.style = function() {
      if ((this.ios_icon_url != null) && this.platforms.indexOf('ios') >= 0) {
        return "background-image: url(" + this.ios_icon_url + ")";
      } else if ((this.android_icon_url != null) && this.platforms.indexOf('android') >= 0) {
        return "background-image: url(" + this.android_icon_url + ")";
      } else {
        return '';
      }
    };
    OtherGame.prototype.html = function() {
      return this.constructor.template(this);
    };
    OtherGame.prototype.getItButtonClassName = function() {
      if (this.platforms.indexOf("" + OF.platform) === -1) {
        return 'store_disabled';
      } else {
        return '';
      }
    };
    OtherGame.prototype.gameClassName = function() {
      var className;
      className = '';
      if (this.platforms.indexOf('android') >= 0) {
        className += 'android_platform ';
      }
      if (this.platforms.indexOf('ios') >= 0) {
        className += 'ios_platform ';
      }
      return className;
    };
    OtherGame.renderOtherGames = function(games) {
      var g, gobj, index, otherGamesHtml, _len;
      this.template = tmpl('othersGamesTmpl');
      otherGamesHtml = '';
      for (index = 0, _len = games.length; index < _len; index++) {
        gobj = games[index];
        if (index >= 20) {
          break;
        }
        g = new OtherGame(gobj.game);
        otherGamesHtml += g.html();
      }
      return otherGamesHtml;
    };
    return OtherGame;
  })();
  this.CommonGame = (function() {
    function CommonGame(game) {
      this.game = game;
      U.extend(this, this.game);
    }
    CommonGame.prototype.style = function() {
      if ((this.android_icon_url != null) && this.platforms.indexOf('android') >= 0) {
        return "background-image: url(" + this.android_icon_url + ")";
      } else if ((this.ios_icon_url != null) && this.platforms.indexOf('ios') >= 0) {
        return "background-image: url(" + this.ios_icon_url + ")";
      } else {
        return '';
      }
    };
    CommonGame.prototype.otherAchievementClassName = function() {
      if (this.other_unlocked_achievement_percent > this.my_unlocked_achievement_percent) {
        return 'higher_achievement';
      } else {
        return '';
      }
    };
    CommonGame.prototype.myAchievementClassName = function() {
      if (this.my_unlocked_achievement_percent > this.other_unlocked_achievement_percent) {
        return 'higher_achievement';
      } else {
        return '';
      }
    };
    CommonGame.prototype.gameClassName = function() {
      var className;
      className = '';
      if (this.platforms.indexOf('android') >= 0) {
        className += 'android_platform ';
      }
      if (this.platforms.indexOf('ios') >= 0) {
        className += 'ios_platform ';
      }
      return className;
    };
    CommonGame.prototype.html = function() {
      return this.constructor.template(this);
    };
    CommonGame.renderCommonGames = function(games) {
      var commonGamesHtml, g, gobj, index, _len;
      this.template = tmpl('sharedGamesTmpl');
      commonGamesHtml = '';
      for (index = 0, _len = games.length; index < _len; index++) {
        gobj = games[index];
        if (index >= 20) {
          break;
        }
        g = new CommonGame(gobj.game);
        commonGamesHtml += g.html();
      }
      return commonGamesHtml;
    };
    return CommonGame;
  })();
}).call(this);
