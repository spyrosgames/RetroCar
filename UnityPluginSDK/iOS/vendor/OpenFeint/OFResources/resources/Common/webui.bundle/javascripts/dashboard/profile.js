// DO NOT EDIT
//   Generated from javascripts/dashboard/profile.coffee
//
(function() {
  var __bind = function(fn, me){ return function(){ return fn.apply(me, arguments); }; };
  this.ProfilePage = (function() {
    function ProfilePage() {
      this.initPageUser();
      this.initPageTitle();
      this.loadFriendStatus();
      this.bindBehaviors();
    }
    ProfilePage.prototype.initPageUser = function() {
      return this.pageUser = new PageUser(OF.page);
    };
    ProfilePage.prototype.initPageTitle = function() {
      if (this.pageUser.isLocalUser) {
        return OF.page.title = 'My Profile';
      } else {
        return OF.page.title = 'Feint Friends';
      }
    };
    ProfilePage.prototype.toggleUpdateStatus = function() {
      var updateStatusNode;
      updateStatusNode = $('#update_status');
      if (this.pageUser.isLocalUser) {
        return updateStatusNode.unhide();
      } else {
        return updateStatusNode.addClass('hidden');
      }
    };
    ProfilePage.prototype.friendsStatusUrl = function() {
      return '/xp/friends/status';
    };
    ProfilePage.prototype.loadFriendStatus = function() {
      if (this.pageUser.isLocalUser) {
        this.friendship = {};
        return this.loadProfileData();
      } else {
        return OF.api(this.friendsStatusUrl(), {
          background: true,
          method: 'GET',
          params: {
            id: this.pageUser.id
          },
          success: __bind(function(data) {
            this.friendship = data.friendship;
            return this.loadProfileData();
          }, this)
        });
      }
    };
    ProfilePage.prototype.profileUrl = function() {
      return "/xp/users/" + this.pageUser.id + "/profile";
    };
    ProfilePage.prototype.loadProfileData = function() {
      return OF.api(this.profileUrl(), {
        success: __bind(function(data) {
          return this.loadProfileDataSuccess(data.user);
        }, this)
      });
    };
    ProfilePage.prototype.loadProfileDataSuccess = function(user) {
      this.pageUser.updatePageUser(user);
      OF.page.sub$('#info_cell').html(this.pageUser.renderBasicInfo());
      this.toggleUpdateStatus();
      this.renderFriendActionArea();
      OF.page.sub$('#game_friend').html(this.pageUser.renderGameFriend());
      this.togglePendingFriend();
      this.toggleSetYourName();
      this.toggerPostMessage();
      this.toggleBlockedOnWall();
      this.toggleFriendOnlyWall();
      this.toggleViewOnlyWall();
      if (!this.friendship.ban_me_by_user_for_wall && this.pageUser.wall_items) {
        this.toggleLoadMore(this.pageUser.wall_items.length);
        this.updateWallInfo(this.pageUser.wall_items, this.pageUser.not_viewed_wall_items_count, true);
      }
      return this.renderMoreOptionsArea();
    };
    ProfilePage.prototype.renderFriendActionArea = function() {
      var friendActionContainer;
      friendActionContainer = $('#friend_action_container');
      friendActionContainer.find('span.user_name').html(this.pageUser.name);
      friendActionContainer.find('span.game_name').html(OF.game.name);
      return this.setFriendActionClassName();
    };
    ProfilePage.prototype.setFriendActionClassName = function() {
      var friendActionContainer, invite, status, userBannedByMe;
      friendActionContainer = $('#friend_action_container');
      friendActionContainer.removeClass();
      status = this.friendship.status;
      userBannedByMe = this.friendship.ban_user_by_me_for_wall;
      invite = this.friendship.can_invite;
      if (userBannedByMe) {
        return friendActionContainer.addClass('unblock_action');
      } else if (status === 'unrelated') {
        return friendActionContainer.addClass('add_action');
      } else if (status === 'pending') {
        return friendActionContainer.addClass('pending_action');
      } else if (status === 'friends' && invite && LocalUser.isFeatureEnabled('invite_to_play_in_profile')) {
        return friendActionContainer.addClass('invite_action');
      }
    };
    ProfilePage.prototype.togglePendingFriend = function() {
      var friendsBadgeNode;
      friendsBadgeNode = OF.page.sub$('#pending_friends');
      if (this.pageUser.isLocalUser && this.pageUser.received_friend_requests_pending_count > 0) {
        return friendsBadgeNode.unhide();
      } else {
        return friendsBadgeNode.addClass('hidden');
      }
    };
    ProfilePage.prototype.toggleSetYourName = function() {
      var setYourNameNode;
      setYourNameNode = $('#set_your_name');
      if (this.pageUser.isLocalUser || LocalUser.getLocalUser().isSetName()) {
        return setYourNameNode.addClass('hidden');
      } else {
        return setYourNameNode.unhide();
      }
    };
    ProfilePage.prototype.toggerPostMessage = function() {
      var postMessageNode;
      postMessageNode = $('#post_message');
      if (this.pageUser.isLocalUser || LocalUser.getLocalUser().isSetName() && this.pageUser.can_post) {
        return postMessageNode.unhide();
      } else {
        return postMessageNode.addClass('hidden');
      }
    };
    ProfilePage.prototype.updateUserInfo = function() {
      var avatarNode, infoCellNode, statusNode, userNameNode;
      if (LocalUser.isLocalUser(this.pageUser.id)) {
        statusNode = $('#status');
        statusNode.html(LocalUser.getLocalUser().status);
        infoCellNode = $('#info_cell');
        userNameNode = infoCellNode.find('.user_name');
        avatarNode = infoCellNode.find('.avatar');
        userNameNode.html(LocalUser.getLocalUser().name);
        return avatarNode.css('background-image', "url(" + (LocalUser.getLocalUser().profile_picture_url) + ")");
      }
    };
    ProfilePage.prototype.toggleBlockedOnWall = function() {
      var blockedOnWall;
      blockedOnWall = $('#blocked_on_wall');
      if (this.friendship.ban_me_by_user_for_wall) {
        return blockedOnWall.unhide();
      } else {
        return blockedOnWall.addClass('hidden');
      }
    };
    ProfilePage.prototype.toggleFriendOnlyWall = function() {
      var friendOnlyWall;
      friendOnlyWall = $('#friend_only_wall');
      if (!this.friendship.ban_me_by_user_for_wall && !this.pageUser.wall_items) {
        friendOnlyWall.unhide();
        return friendOnlyWall.find('span.user_name').html(this.pageUser.name);
      } else {
        return friendOnlyWall.addClass('hidden');
      }
    };
    ProfilePage.prototype.toggleViewOnlyWall = function() {
      var viewOnlyWall;
      viewOnlyWall = $('#view_only_wall');
      if (this.friendship.ban_me_by_user_for_wall || !this.pageUser.wall_items || this.pageUser.can_post) {
        return viewOnlyWall.addClass('hidden');
      } else {
        viewOnlyWall.unhide();
        return viewOnlyWall.find('span.user_name').html(this.pageUser.name);
      }
    };
    ProfilePage.prototype.toggleLoadMore = function(wallItemsLength) {
      var loadMore;
      loadMore = $('#load_more');
      if (wallItemsLength > 10) {
        return loadMore.unhide();
      } else {
        return loadMore.addClass('hidden');
      }
    };
    ProfilePage.prototype.updateWallInfo = function(wallItems, newItemNum, positionFlag) {
      var messagesContainerNode, newMessagesHTML;
      messagesContainerNode = $('#messages_container');
      messagesContainerNode.find('li.new_message').removeClass('new_message');
      newMessagesHTML = Wall.renderWalls(wallItems, newItemNum, this.pageUser);
      if (positionFlag) {
        return messagesContainerNode.append(newMessagesHTML);
      } else {
        return messagesContainerNode.prepend(newMessagesHTML);
      }
    };
    ProfilePage.prototype.renderMoreOptionsArea = function() {
      this.toggleMoreOptions();
      if (!this.pageUser.isLocalUser) {
        this.toggleRemoveFriend();
        this.toggleBlockUser();
        return this.setMoreActionClassName();
      }
    };
    ProfilePage.prototype.toggleMoreOptions = function() {
      var moreOptionsContainer;
      moreOptionsContainer = $('#more_options');
      if (this.pageUser.isLocalUser) {
        return moreOptionsContainer.addClass('hidden');
      } else {
        return moreOptionsContainer.unhide();
      }
    };
    ProfilePage.prototype.toggleRemoveFriend = function() {
      var removeFriendContainer;
      removeFriendContainer = $('#remove_friend');
      if (this.friendship.status === 'friends') {
        return removeFriendContainer.unhide();
      } else {
        return removeFriendContainer.addClass('hidden');
      }
    };
    ProfilePage.prototype.toggleBlockUser = function() {
      var blockUserContainer;
      blockUserContainer = $('#block_user');
      if (this.friendship.ban_user_by_me_for_wall) {
        return blockUserContainer.addClass('hidden');
      } else {
        return blockUserContainer.unhide();
      }
    };
    ProfilePage.prototype.setMoreActionClassName = function() {
      var columnNum, status, userBannedByMe;
      columnNum = 3;
      status = this.friendship.status;
      userBannedByMe = this.friendship.ban_user_by_me_for_wall;
      if (!status === 'friends') {
        columnNum -= 1;
      }
      if (userBannedByMe) {
        columnNum -= 1;
      } else {

      }
      return $('#more_options').find('.more_action').addClass("col" + columnNum);
    };
    ProfilePage.prototype.bindBehaviors = function() {
      if (this.pageUser.isLocalUser) {
        $('div.status_container').touch(__bind(function() {
          return this.updateStatusListener();
        }, this));
      }
      $('#setting_icon').touch(__bind(function() {
        return this.openSettingsPage();
      }, this));
      $('#add_friend').find('.action_button').touch(__bind(function() {
        return this.addFriendListener();
      }, this));
      $('#invite_play').find('.action_button').touch(__bind(function() {
        return this.invitePlayListener();
      }, this));
      $('#unblock_user').find('.action_button').touch(__bind(function() {
        return this.unblockUserListener();
      }, this));
      $('#user_profile #games').touch(__bind(function() {
        return this.gamesListener();
      }, this));
      $('#user_profile #friends').touch(__bind(function() {
        return this.friendsListener();
      }, this));
      $('#set_name').touch(__bind(function() {
        return this.setNameListener();
      }, this));
      $('#post_message').find('form').submit(__bind(function() {
        return this.postMessageSubmitListener();
      }, this));
      $('#post').touch(__bind(function() {
        return this.postListener();
      }, this));
      $('div.delete_message').touch(__bind(function(event) {
        return this.deleteMessageListener(event);
      }, this));
      $('#load_more').touch(__bind(function() {
        return this.loadMoreListener();
      }, this));
      $('#more_title').touch(__bind(function() {
        return this.moreTitleListener();
      }, this));
      $('#remove_friend').find('.badge_button').touch(__bind(function() {
        return this.removeFriendListener();
      }, this));
      $('#block_user').find('.badge_button').touch(__bind(function() {
        return this.blockUserListener();
      }, this));
      $('#report_user').find('.badge_button').touch(__bind(function() {
        return this.reportUserListener();
      }, this));
      return $('span.invite_game').touch(__bind(function(event) {
        return this.inviteGameListener(event);
      }, this));
    };
    ProfilePage.prototype.updateStatusListener = function() {
      return OF.push('dashboard/statusupdate');
    };
    ProfilePage.prototype.openSettingsPage = function() {
      return OF.action('openSettings');
    };
    ProfilePage.prototype.addFriendLog = function() {
      return '/webui/dashboard/profile_addFriend';
    };
    ProfilePage.prototype.addFriendUrl = function() {
      return '/xp/friend_requests';
    };
    ProfilePage.prototype.addFriendListener = function() {
      OF.GA.page(this.addFriendLog());
      OF.api(this.addFriendUrl(), {
        background: true,
        method: 'POST',
        params: {
          friend_id: this.pageUser.id
        },
        failure: function() {
          return OF.alert('Oops', 'Oops! An error has occured.');
        }
      });
      this.friendship.status = 'pending';
      return this.renderFriendActionArea();
    };
    ProfilePage.prototype.invitePlayLog = function() {
      return '/webui/dashboard/profile_invitePlay';
    };
    ProfilePage.prototype.invitePlayListener = function() {
      OF.GA.page(this.invitePlayLog());
      return this.postInvitation();
    };
    ProfilePage.prototype.unblockUserLog = function() {
      return '/webui/dashboard/profile_unblockUser';
    };
    ProfilePage.prototype.unblockUserUrl = function() {
      return "/xp/wall/banned_users/" + this.pageUser.id;
    };
    ProfilePage.prototype.unblockUserListener = function() {
      OF.GA.page(this.unblockUserLog());
      return OF.api(this.unblockUserUrl(), {
        background: true,
        method: 'DELETE',
        success: function() {
          return OF.refresh();
        },
        failure: function() {
          return OF.alert('Oops', 'Oops! An error has occured.');
        }
      });
    };
    ProfilePage.prototype.gamesUrl = function() {
      if (this.pageUser.isLocalUser || !LocalUser.isFeatureEnabled('new_others_game_list')) {
        return 'dashboard/me/games';
      } else {
        return 'dashboard/games';
      }
    };
    ProfilePage.prototype.gamesListener = function() {
      return OF.push(this.gamesUrl(), {
        params: {
          user_id: this.pageUser.id,
          user_name: this.pageUser.name
        }
      });
    };
    ProfilePage.prototype.myFriendsUrl = function() {
      return 'dashboard/me/friends';
    };
    ProfilePage.prototype.commonFriendsUrl = function() {
      return 'dashboard/friends';
    };
    ProfilePage.prototype.friendsListener = function() {
      if (this.pageUser.isLocalUser) {
        return OF.push(this.myFriendsUrl());
      } else {
        return OF.push(this.commonFriendsUrl(), {
          params: {
            user: this.pageUser
          }
        });
      }
    };
    ProfilePage.prototype.setNameUrl = function() {
      return 'settings/profile_configuration';
    };
    ProfilePage.prototype.setNameBackUrl = function() {
      return "dashboard/profile?user_id=" + this.pageUser.id;
    };
    ProfilePage.prototype.setNameListener = function() {
      return OF.push(this.setNameUrl(), {
        params: {
          back_url: this.setNameBackUrl()
        }
      });
    };
    ProfilePage.prototype.postMessageLog = function() {
      return '/webui/dashboard/profile_postComment';
    };
    ProfilePage.prototype.preventInject = function(message) {
      return message.replace(/</gi, "&lt;").replace(/>/gi, "&gt;");
    };
    ProfilePage.prototype.postMessageSubmitListener = function() {
      var newMessage;
      newMessage = $.trim($('#new_message').val());
      if (newMessage.length > 0) {
        newMessage = this.preventInject(newMessage);
        OF.GA.page(this.postMessageLog());
        this.postMessage(newMessage);
      } else {
        OF.alert('Missing Data', 'You haven\'t input anything to update yet.');
      }
      return false;
    };
    ProfilePage.prototype.postMessage = function(newMessage) {
      var firstMessageNode, messageContentNode;
      firstMessageNode = $('li.message:first');
      messageContentNode = firstMessageNode.find('.center_column');
      return this.postToWall('status_messages', {
        status_message: {
          body: newMessage
        },
        last_item_id: messageContentNode.data('id')
      }, function() {
        return $('#new_message').val('');
      }, __bind(function(data) {
        return $('#post_message').addClass('hidden');
      }, this));
    };
    ProfilePage.prototype.postInvitation = function() {
      var firstMessageNode, messageContentNode;
      firstMessageNode = $('li.message:first');
      messageContentNode = firstMessageNode.find('.center_column');
      return this.postToWall('game_invitations', {
        last_item_id: messageContentNode.data('id')
      }, __bind(function() {
        $('#friend_action_container').removeClass();
        return OF.alert("Awesome! You just invited " + this.pageUser.name + " to play " + OF.game.name);
      }, this), __bind(function(data) {
        return $('#friend_action_container').removeClass();
      }, this));
    };
    ProfilePage.prototype.postToWallUrl = function(subpath) {
      return "/xp/walls/" + this.pageUser.wall_id + "/" + subpath;
    };
    ProfilePage.prototype.postToWall = function(subpath, paramObj, successCallback, on406Callback) {
      var firstMessageNode, messageContentNode;
      firstMessageNode = $('li.message:first');
      messageContentNode = firstMessageNode.find('.center_column');
      return OF.api(this.postToWallUrl(subpath), {
        method: 'POST',
        params: paramObj,
        success: __bind(function(data) {
          this.updateWallInfo(data.wall_items, data.wall_items.length, false);
          return successCallback();
        }, this),
        on406: function(data) {
          on406Callback(data);
          return OF.alert("No access", $.htmlDecode(data.exception.message));
        },
        failure: function(data, status) {
          if (!status.match(/^406/)) {
            OF.alert('Oops', 'Oops! An error has occured.');
            return false;
          }
        }
      });
    };
    ProfilePage.prototype.postListener = function() {
      return $('#post_message').find('form').submit();
    };
    ProfilePage.prototype.deleteMessageConfirmLog = function() {
      return '/webui/dashboard/profile_confirmDeleteWallItem';
    };
    ProfilePage.prototype.deleteMessageCancelLog = function() {
      return '/webui/dashboard/profile_cancelDeleteWallItem';
    };
    ProfilePage.prototype.deleteMessageUrl = function(messageContentNode) {
      return "/xp/walls/" + this.pageUser.wall_id + "/items/" + (messageContentNode.data('id'));
    };
    ProfilePage.prototype.deleteMessageListener = function(event) {
      var currentNode, messageNode;
      currentNode = $(event.target);
      messageNode = currentNode.parents('.message');
      return OF.confirm('', 'Do you really want to delete this item?', 'Yes', 'No', __bind(function() {
        var messageContentNode;
        OF.GA.page(this.deleteMessageConfirmLog());
        messageContentNode = messageNode.find('.center_column');
        return OF.api(this.deleteMessageUrl(messageContentNode), {
          method: 'DELETE',
          success: function() {
            return messageNode.remove();
          },
          failure: function() {
            return OF.alert('Oops', 'Oops! An error has occured.');
          }
        });
      }, this), __bind(function() {
        return OF.GA.page(this.deleteMessageCancelLog());
      }, this));
    };
    ProfilePage.prototype.loadMoreLog = function() {
      return '/webui/dashboard/profile_loadMoreMessage';
    };
    ProfilePage.prototype.loadMoreUrl = function() {
      return "/xp/walls/" + this.pageUser.wall_id + "/items/more";
    };
    ProfilePage.prototype.loadMoreListener = function() {
      var lastMessageNode, messageContentNode;
      OF.GA.page(this.loadMoreLog());
      lastMessageNode = $('li.message:last');
      messageContentNode = lastMessageNode.find('.center_column');
      return OF.api(this.loadMoreUrl(), {
        method: 'GET',
        params: {
          last_item_id: messageContentNode.data('id'),
          wall_id: this.pageUser.wall_id
        },
        success: __bind(function(data) {
          this.toggleLoadMore(data.wall_items.length);
          return this.updateWallInfo(data.wall_items, 0, true);
        }, this),
        on406: __bind(function(data) {
          OF.alert("No access", $.htmlDecode(data.exception.message));
          $('li.message').addClass('hidden');
          return this.toggleLoadMore(0);
        }, this),
        failure: function(data, status) {
          if (!status.match(/^406/)) {
            OF.alert('Oops', 'Oops! An error has occured.');
            return false;
          }
        }
      });
    };
    ProfilePage.prototype.moreTitleListener = function() {
      return $('#more_title').toggleClass('expanded').next('.more_action').toggleClass('hidden');
    };
    ProfilePage.prototype.removeFriendConfirmLog = function() {
      return '/webui/dashboard/profile_confirmRemoveFriend';
    };
    ProfilePage.prototype.removeFriendCancelLog = function() {
      return '/webui/dashboard/profile_cancelRemoveFriend';
    };
    ProfilePage.prototype.removeFriendUrl = function() {
      return "/xp/friends/" + this.pageUser.id;
    };
    ProfilePage.prototype.removeFriendListener = function() {
      return OF.confirm('', 'Are you sure you want to remove this user as a friend?', 'Yes', 'No', __bind(function() {
        OF.GA.page(this.removeFriendConfirmLog());
        return OF.api(this.removeFriendUrl(), {
          background: true,
          method: 'DELETE',
          success: function() {
            return OF.refresh();
          },
          failure: function() {
            return OF.alert('Oops', 'Oops! An error has occured.');
          }
        });
      }, this), __bind(function() {
        return OF.GA.page(this.removeFriendCancelLog());
      }, this));
    };
    ProfilePage.prototype.blockUserConfirmLog = function() {
      return '/webui/dashboard/profile_confirmBlockUser';
    };
    ProfilePage.prototype.blockUserCancelLog = function() {
      return '/webui/dashboard/profile_cancelBlockUser';
    };
    ProfilePage.prototype.blockUserUrl = function() {
      return '/xp/wall/banned_users';
    };
    ProfilePage.prototype.blockUserListener = function() {
      return OF.confirm('', 'Are you sure you want to block this user so that this user won\'t be able to view your wall?', 'Yes', 'No', __bind(function() {
        OF.GA.page(this.blockUserConfirmLog());
        return OF.api(this.blockUserUrl(), {
          background: true,
          method: 'POST',
          params: {
            user_id: this.pageUser.id
          },
          success: function() {
            return OF.refresh();
          },
          failure: function() {
            return OF.alert('Oops', 'Oops! An error has occured.');
          }
        });
      }, this), __bind(function() {
        return OF.GA.page(this.blockUserCancelLog());
      }, this));
    };
    ProfilePage.prototype.reportUserConfirmLog = function() {
      return '/webui/dashboard/profile_confirmReportUser';
    };
    ProfilePage.prototype.reportUserCancelLog = function() {
      return '/webui/dashboard/profile_cancelReportUser';
    };
    ProfilePage.prototype.reportUserUrl = function() {
      return "/xp/users/" + this.pageUser.id + "/abuse_flags";
    };
    ProfilePage.prototype.reportUserListener = function() {
      return OF.confirm('', 'Are you sure you want to submit an abuse report?', 'Yes, Report', 'Cancel', __bind(function() {
        OF.GA.page(this.reportUserConfirmLog());
        return OF.api(this.reportUserUrl(), {
          background: true,
          method: 'POST',
          params: {
            abuse_type: 'wall'
          },
          on406: function(data) {
            return OF.alert("Report forbidden", $.htmlDecode(data.exception.message));
          },
          failure: function(data, status) {
            if (!status.match(/^406/)) {
              OF.alert('Oops', 'Oops! An error has occured.');
              return false;
            }
          }
        });
      }, this), __bind(function() {
        return OF.GA.page(this.reportUserCancelLog());
      }, this));
    };
    ProfilePage.prototype.openMarketLog = function() {
      return '/webui/dashboard/profile_openMarket?game_name=';
    };
    ProfilePage.prototype.inviteGameListener = function(event) {
      var node;
      node = $(event.target);
      OF.GA.page("" + (this.openMarketLog()) + (node.html()));
      return OF.action('openMarket', {
        package_name: node.data('packagename')
      });
    };
    return ProfilePage;
  })();
}).call(this);
