// DO NOT EDIT
//   Generated from javascripts/dashboard/statusupdate.coffee
//
(function() {
  var __bind = function(fn, me){ return function(){ return fn.apply(me, arguments); }; };
  this.StatusUpdate = (function() {
    function StatusUpdate() {
      this.localUser = LocalUser.getLocalUser();
      this.bindBehaviors();
    }
    StatusUpdate.prototype.bindBehaviors = function() {
      $('#update_status').submit(__bind(function() {
        return this.updateStatusListener();
      }, this));
      return $('#update').touch(__bind(function() {
        return this.updateListener();
      }, this));
    };
    StatusUpdate.prototype.updateStatusLog = function() {
      return '/webui/dashboard/statusupdate_updateButton';
    };
    StatusUpdate.prototype.updateStatusUrl = function() {
      return "/xp/users/" + this.localUser.id;
    };
    StatusUpdate.prototype.updateStatusListener = function() {
      var newStatus, newStatusNode;
      newStatusNode = $('#new_status');
      newStatus = $.trim(newStatusNode.val());
      if (newStatus.length > 0) {
        OF.GA.page(this.updateStatusLog());
        newStatus = newStatus.replace(/</gi, "&lt;").replace(/>/gi, "&gt;");
        OF.api(this.updateStatusUrl(), {
          method: 'PUT',
          loader: '#update',
          params: {
            user: {
              status: newStatus
            }
          },
          success: __bind(function() {
            OF.user.status = newStatus;
            return OF.goBack();
          }, this),
          failure: function() {
            return OF.alert('Oops', 'Oops! An error has occured.');
          }
        });
      } else {
        OF.alert('Missing Data', 'You haven\'t input anything to update yet.');
      }
      return false;
    };
    StatusUpdate.prototype.updateListener = function() {
      return $('#update_status').submit();
    };
    return StatusUpdate;
  })();
}).call(this);
