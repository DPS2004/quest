(function () {
  var remote = require('remote');
  var app = remote.require('app');
  window.menuClick = window.menuClick || {};
  
  var fileOpen = function () {
    window.menuClick.openFile();
  };

  var fileSave = function () {
    window.menuClick.saveFile();
  };

  var fileSaveAs = function () {
    window.menuClick.saveFileAs();
  };

  var editCopy = function () {
    window.menuClick.copy();
  };

  var editSelectAll = function () {
    window.menuClick.selectAll();
  };

  var about = function () {
    window.menuClick.about();
  };
  
  var settings = function () {
    window.menuClick.settings();
  };

  var template;

  if (process.platform === 'darwin') {
    template = [
      {
        label: 'Quest Player',
        submenu: [
          {
            label: 'About Quest Player',
            click: about
          },
          {
            type: 'separator'
          },
          {
            label: 'Preferences...',
            accelerator: 'Command+,',
            click: settings
          },
          {
            type: 'separator'
          },
          {
            label: 'Services',
            submenu: []
          },
          {
            type: 'separator'
          },
          {
            label: 'Hide Quest Player',
            accelerator: 'Command+H',
            selector: 'hide:'
          },
          {
            label: 'Hide Others',
            accelerator: 'Command+Shift+H',
            selector: 'hideOtherApplications:'
          },
          {
            label: 'Show All',
            selector: 'unhideAllApplications:'
          },
          {
            type: 'separator'
          },
          {
            label: 'Quit',
            accelerator: 'Command+Q',
            selector: 'terminate:'
          },
        ]
      },
      {
        label: 'File',
        submenu: [
          {
            label: 'Open...',
            accelerator: 'Command+O',
            click: fileOpen
          },
          {
            label: 'Save',
            accelerator: 'Command+S',
            click: fileSave
          },
          {
            label: 'Save As...',
            accelerator: 'Command+Shift+S',
            click: fileSaveAs
          },
        ]
      },
      {
        label: 'Edit',
        submenu: [
          {
            label: 'Select All',
            accelerator: 'Command+A',
            click: editSelectAll
          },
          {
            label: 'Copy',
            accelerator: 'Command+C',
            click: editCopy
          },
        ]
      },
      {
        label: 'View',
        submenu: [
          {
            label: 'Reload',
            accelerator: 'Alt+Command+R',
            click: function() { remote.getCurrentWindow().reloadIgnoringCache(); }
          },
          {
            label: 'Toggle DevTools',
            accelerator: 'Alt+Command+I',
            click: function() { remote.getCurrentWindow().toggleDevTools(); }
          },
        ]
      },
      {
        label: 'Window',
        submenu: [
          {
            label: 'Minimize',
            accelerator: 'Command+M',
            selector: 'performMiniaturize:'
          },
          {
            label: 'Close',
            accelerator: 'Command+W',
            selector: 'performClose:'
          },
          {
            type: 'separator'
          },
          {
            label: 'Bring All to Front',
            selector: 'arrangeInFront:'
          },
        ]
      },
    ];
  }
  else {
    template = [
      {
        label: '&File',
        submenu: [
          {
            label: '&Open...',
            accelerator: 'Ctrl+O',
            click: fileOpen
          },
          {
            label: '&Save',
            accelerator: 'Ctrl+S',
            click: fileSave
          },
          {
            label: 'Save &As...',
            accelerator: 'Ctrl+Shift+S',
            click: fileSaveAs
          },
          {
            type: 'separator'
          },
          {
            label: 'E&xit',
            click: function() { app.quit(); }
          }
        ]
      },
      {
        label: '&Edit',
        submenu: [
          {
            label: 'Select &All',
            accelerator: 'Ctrl+A',
            click: editSelectAll
          },
          {
            label: '&Copy',
            accelerator: 'Ctrl+C',
            click: editCopy
          },
        ]
      },
      {
        label: '&View',
        submenu: [
          {
            label: '&Reload',
            accelerator: 'Ctrl+Shift+R',
            click: function() { remote.getCurrentWindow().reloadIgnoringCache(); }
          },
          {
            label: '&Toggle DevTools',
            accelerator: 'F12',
            click: function() { remote.getCurrentWindow().toggleDevTools(); }
          },
        ]
      },
    ];
  }

  var Menu = remote.require('menu');
  var menu = Menu.buildFromTemplate(template);
  Menu.setApplicationMenu(menu);
})();