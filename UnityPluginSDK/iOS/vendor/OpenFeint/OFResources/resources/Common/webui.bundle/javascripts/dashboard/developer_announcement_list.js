// DO NOT EDIT
//   Generated from javascripts/dashboard/developer_announcement_list.coffee
//
(function() {
  var __bind = function(fn, me){ return function(){ return fn.apply(me, arguments); }; };
  this.DeveloperAnnouncementListPage = (function() {
    function DeveloperAnnouncementListPage() {
      this.replaceHeader();
      this.loadDeveloperAnnouncementList();
    }
    DeveloperAnnouncementListPage.prototype.replaceHeader = function() {
      if ($('#header_replacement').length > 0) {
        $('#header').remove();
        return $('#header_replacement').attr('id', 'header');
      }
    };
    DeveloperAnnouncementListPage.prototype.announcementUrl = function() {
      return "/xp/games/" + OF.game.id + "/announcements";
    };
    DeveloperAnnouncementListPage.prototype.loadDeveloperAnnouncementList = function() {
      return OF.api(this.announcementUrl(), {
        loader: "#loader",
        success: __bind(function(data) {
          return this.renderDevelopAnnouncementList(data.announcements);
        }, this)
      });
    };
    DeveloperAnnouncementListPage.prototype.toggleLoadMore = function(announceListLength) {
      var loadMoreButton;
      loadMoreButton = $('#load_more');
      if (announceListLength > DeveloperAnnouncement.announcePerPage) {
        return loadMoreButton.unhide();
      } else {
        return loadMoreButton.addClass('hidden');
      }
    };
    DeveloperAnnouncementListPage.prototype.renderDevelopAnnouncementList = function(announcements) {
      var loadMoreButton, loaderNode;
      loaderNode = $('#loader');
      loadMoreButton = $('#load_more');
      return $('#announce_list').append(DeveloperAnnouncement.renderAnnouncementList(announcements), loaderNode, loadMoreButton);
    };
    return DeveloperAnnouncementListPage;
  })();
}).call(this);
