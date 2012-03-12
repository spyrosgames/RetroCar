// DO NOT EDIT
//   Generated from javascripts/dashboard/developer_announcement.coffee
//
(function() {
  var __bind = function(fn, me){ return function(){ return fn.apply(me, arguments); }; };
  this.DeveloperAnnouncementPage = (function() {
    function DeveloperAnnouncementPage() {
      this.replaceHeader();
      this.from_dashboard = OF.page.params.from_dashboard;
      this.announcementId = OF.page.params.id;
      this.toggleViewAll();
      this.loadDeveloperAnnouncement();
    }
    DeveloperAnnouncementPage.prototype.replaceHeader = function() {
      if ($('#header_replacement').length > 0) {
        $('#header').remove();
        return $('#header_replacement').attr('id', 'header');
      }
    };
    DeveloperAnnouncementPage.prototype.toggleViewAll = function() {
      var viewAllNode;
      viewAllNode = $('#view_all');
      if (this.from_dashboard) {
        return viewAllNode.unhide();
      } else {
        return viewAllNode.addClass('hidden');
      }
    };
    DeveloperAnnouncementPage.prototype.announcementUrl = function() {
      return "/xp/games/" + OF.game.id + "/announcements/" + this.announcementId;
    };
    DeveloperAnnouncementPage.prototype.loadDeveloperAnnouncement = function() {
      return OF.api(this.announcementUrl(), {
        success: __bind(function(data) {
          return this.renderDevelopAnnouncement(data.announcement);
        }, this)
      });
    };
    DeveloperAnnouncementPage.prototype.renderDevelopAnnouncement = function(announcement) {
      return $('#announcement').html(DeveloperAnnouncement.renderSingleAnnouncement(announcement));
    };
    return DeveloperAnnouncementPage;
  })();
}).call(this);
