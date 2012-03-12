// DO NOT EDIT
//   Generated from javascripts/settings/profile_configuration.coffee
//
(function() {
  var avatarNode, backUrl, nextStep, profileNameForm, usernameNode;
  usernameNode = $('#user_name');
  avatarNode = $('.avatar');
  profileNameForm = $('#profile_name');
  backUrl = OF.page.params.back_url;
  OF.api("/xp/users/me", {
    success: function(data) {
      OF.user = data.user;
      if (OF.user.profile_picture_url) {
        avatarNode.css('background-image', "url(" + OF.user.profile_picture_url + ")");
      }
      if (OF.page.tempUserName != null) {
        OF.page.resumeUsername(OF.page.tempUserName);
      } else if (OF.user.has_set_name) {
        usernameNode.val(OF.user.name);
        OF.page.originalUserName = OF.user.name;
      }
      if (OF.user.email && OF.user.email.length) {
        $("#OKay").unhide();
        return $("#Next").hide();
      } else {
        $("#OKay").hide();
        return $("#Next").unhide();
      }
    }
  });
  OF.page.resumeUsername = function(username) {
    return usernameNode.val(username);
  };
  OF.page.onProfilePictureChanged = function(url) {
    OF.page.resumeUsername(OF.page.tempUserName);
    avatarNode.css('background-image', "url(" + url + "?" + (Date.now()) + ")");
    return OF.user.profile_picture_url = url;
  };
  $('.avatar, #select_photo').touch(function() {
    OF.page.tempUserName = usernameNode.val();
    OF.GA.page('/webui/dashboard/profile_configuration_selectPhoto');
    OF.sendAction('profilePicture', {
      callback: 'OF.page.onProfilePictureChanged'
    });
  });
  nextStep = function() {
    if (OF.user.email && OF.user.email.length) {
      if (backUrl != null) {
        return OF.push(backUrl);
      } else {
        return OF.goBack();
      }
    } else {
      return setTimeout(function() {
        if (backUrl != null) {
          return OF.push('settings/profile_secure', {
            params: {
              back_url: backUrl
            }
          });
        } else {
          return OF.push('settings/profile_secure');
        }
      }, 300);
    }
  };
  profileNameForm.submit(function() {
    var username;
    OF.page.tempUserName = null;
    username = $.trim(usernameNode.val());
    if (username === OF.page.originalUserName) {
      nextStep();
    } else if (username.length > 0) {
      OF.api("/xp/users/" + OF.user.id, {
        method: 'PUT',
        loader: '#OKay, #Next',
        params: {
          user: {
            name: username,
            has_set_name: 1
          }
        },
        success: function() {
          OF.user.name = username;
          OF.user.has_set_name = true;
          OF.page.user = OF.user;
          nextStep();
          return OF.alert('Success', "Your profile page has been modified");
        },
        failure: function() {
          return OF.alert('Oops', 'Looks like that user name is already taken.');
        }
      });
    } else {
      OF.alert('Missing Data', 'Enter a new username in order to change your name.');
    }
    return false;
  });
  $("#OKay, #Next").touch(function() {
    OF.GA.page('/webui/dashboard/profile_configuration_changeName');
    return profileNameForm.submit();
  });
}).call(this);
