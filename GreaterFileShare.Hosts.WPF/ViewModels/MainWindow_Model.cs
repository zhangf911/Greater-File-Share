﻿using System.Reactive;
using System.Reactive.Linq;
using MVVMSidekick.ViewModels;
using MVVMSidekick.Views;
using MVVMSidekick.Reactive;
using MVVMSidekick.Services;
using MVVMSidekick.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using GreaterFileShare.Hosts.WPF.Models;
using GreaterFileShare.Hosts.Core;
using Microsoft.WindowsAPICodePack.Dialogs;
using GreaterFileShare.Hosts.WPF.Services;

namespace GreaterFileShare.Hosts.WPF.ViewModels
{

    public class MainWindow_Model : ViewModelBase<MainWindow_Model>
    {
        // If you have install the code sniplets, use "propvm + [tab] +[tab]" create a property propcmd for command
        // 如果您已经安装了 MVVMSidekick 代码片段，请用 propvm +tab +tab 输入属性 propcmd 输入命令

        //IDisposable _currentTaskListening;
        public MainWindow_Model()
        {

            if (IsInDesignMode)
            {

                Messages.Add(new Models.MessageEntry { Time = DateTime.Now, Message = "Message Here" });
                return;
            }
            GreaterFileShare.Hosts.WPF.Services.FileSystemHubService.vmInstance = this;
            var source1 = GlobalEventRouter.GetEventChannel<Exception>()
                    .Select(x =>
                       {
                           Exception[] rval = new[] { x.EventData };
                           if (x.EventData is AggregateException)
                           {
                               
                               return rval.Union((x.EventData as AggregateException).InnerExceptions);

                           }
                           return rval;
                       })
                       .SelectMany(x=>x)
                       .Select(x=>x.Message);

            var source2 = GlobalEventRouter.GetEventChannel<string>()
                    .Where(x => x.EventName == "Logging")
                    .Select(x => x.EventData);
            var source3 = new WebLoggingSource();

            new[] { source1, source2, source3 }
                .ToObservable()
                .SelectMany(ms => ms)
                .Select(x => new MessageEntry { Time = DateTime.Now, Message = x })
                .ObserveOnDispatcher()
                .Subscribe(x =>
                    {
                        Messages.Add(x);
                        if (Messages.Count > 500)
                        {
                            Messages.RemoveAt(0);
                        }

                        CurrentMessageIndex = Messages.Count - 1;
                    })
                .DisposeWith(this);
            //CurrentTask.DisposeWith(this);



        }



        public ObservableCollection<MessageEntry> Messages
        {
            get { return _MessagesLocator(this).Value; }
            set { _MessagesLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property ObservableCollection<MessageEntry> Messages Setup        
        protected Property<ObservableCollection<MessageEntry>> _Messages = new Property<ObservableCollection<MessageEntry>> { LocatorFunc = _MessagesLocator };
        static Func<BindableBase, ValueContainer<ObservableCollection<MessageEntry>>> _MessagesLocator = RegisterContainerLocator<ObservableCollection<MessageEntry>>(nameof(Messages), model => model.Initialize(nameof(Messages), ref model._Messages, ref _MessagesLocator, _MessagesDefaultValueFactory));
        static Func<ObservableCollection<MessageEntry>> _MessagesDefaultValueFactory = () => new ObservableCollection<MessageEntry>();
        #endregion



        public int CurrentMessageIndex
        {
            get { return _CurrentMessageIndexLocator(this).Value; }
            set { _CurrentMessageIndexLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property int CurrentMessageIndex Setup        
        protected Property<int> _CurrentMessageIndex = new Property<int> { LocatorFunc = _CurrentMessageIndexLocator };
        static Func<BindableBase, ValueContainer<int>> _CurrentMessageIndexLocator = RegisterContainerLocator<int>(nameof(CurrentMessageIndex), model => model.Initialize(nameof(CurrentMessageIndex), ref model._CurrentMessageIndex, ref _CurrentMessageIndexLocator, _CurrentMessageIndexDefaultValueFactory));
        static Func<int> _CurrentMessageIndexDefaultValueFactory = () => default(int);
        #endregion



        public ShareFileTask CurrentTask
        {
            get { return _CurrentTaskLocator(this).Value; }
            set { _CurrentTaskLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property ShareFileTask CurrentTask Setup        
        protected Property<ShareFileTask> _CurrentTask = new Property<ShareFileTask> { LocatorFunc = _CurrentTaskLocator };
        static Func<BindableBase, ValueContainer<ShareFileTask>> _CurrentTaskLocator = RegisterContainerLocator<ShareFileTask>(nameof(CurrentTask), model => model.Initialize(nameof(CurrentTask), ref model._CurrentTask, ref _CurrentTaskLocator, _CurrentTaskDefaultValueFactory));
        static Func<ShareFileTask> _CurrentTaskDefaultValueFactory = () => new ShareFileTask();
        #endregion





        public ObservableCollection<ShareFileTask> HostingTasks
        {
            get { return _HostingTasksLocator(this).Value; }
            set { _HostingTasksLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property ObservableCollection<ShareFileTask> HostingTasks Setup        
        protected Property<ObservableCollection<ShareFileTask>> _HostingTasks = new Property<ObservableCollection<ShareFileTask>> { LocatorFunc = _HostingTasksLocator };
        static Func<BindableBase, ValueContainer<ObservableCollection<ShareFileTask>>> _HostingTasksLocator = RegisterContainerLocator<ObservableCollection<ShareFileTask>>(nameof(HostingTasks), model => model.Initialize(nameof(HostingTasks), ref model._HostingTasks, ref _HostingTasksLocator, _HostingTasksDefaultValueFactory));
        static Func<ObservableCollection<ShareFileTask>> _HostingTasksDefaultValueFactory = () => new ObservableCollection<ShareFileTask>();
        #endregion





        public CommandModel<ReactiveCommand, String> CommandNewHost
        {
            get { return _CommandNewHostLocator(this).Value; }
            set { _CommandNewHostLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property CommandModel<ReactiveCommand, String> CommandNewHost Setup        

        protected Property<CommandModel<ReactiveCommand, String>> _CommandNewHost = new Property<CommandModel<ReactiveCommand, String>> { LocatorFunc = _CommandNewHostLocator };
        static Func<BindableBase, ValueContainer<CommandModel<ReactiveCommand, String>>> _CommandNewHostLocator = RegisterContainerLocator<CommandModel<ReactiveCommand, String>>("CommandNewHost", model => model.Initialize("CommandNewHost", ref model._CommandNewHost, ref _CommandNewHostLocator, _CommandNewHostDefaultValueFactory));
        static Func<BindableBase, CommandModel<ReactiveCommand, String>> _CommandNewHostDefaultValueFactory =
            model =>
            {
                var resource = "CommandNewHost";           // Command resource  
                var commandId = "CommandNewHost";
                var vm = CastToCurrentType(model);
                var cmd = new ReactiveCommand(canExecute: true) { ViewModel = model }; //New Command Core

                cmd.DoExecuteUIBusyTask(
                        vm,
                        async e =>
                        {
                            var t = new ShareFileTask();
                            if (vm.HostingTasks.Count > 0)
                            {
                                var hs = new HashSet<int>(vm.HostingTasks.Select(x => x.Port).Where(x => x.HasValue).Select(x => x.Value));
                                t.Port = vm.HostingTasks.Max(x => x.Port ?? 0) + 1;
                                while (hs.Contains(t.Port.Value))
                                {
                                    t.Port++;
                                    if (t.Port > 65535)
                                    {
                                        t.Port = 0;
                                    }
                                }
                            }
                            else
                            {
                                t.Port = 8080;
                            }
                            vm.HostingTasks.Add(t);
                            vm.CurrentTask = t;

                            await MVVMSidekick.Utilities.TaskExHelper.Yield();
                        })
                    .DoNotifyDefaultEventRouter(vm, commandId)
                    .Subscribe()
                    .DisposeWith(vm);

                var cmdmdl = cmd.CreateCommandModel(resource);


                return cmdmdl;
            };

        #endregion


        public CommandModel<ReactiveCommand, String> CommandDeleteHost
        {
            get { return _CommandDeleteHostLocator(this).Value; }
            set { _CommandDeleteHostLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property CommandModel<ReactiveCommand, String> CommandDeleteHost Setup        

        protected Property<CommandModel<ReactiveCommand, String>> _CommandDeleteHost = new Property<CommandModel<ReactiveCommand, String>> { LocatorFunc = _CommandDeleteHostLocator };
        static Func<BindableBase, ValueContainer<CommandModel<ReactiveCommand, String>>> _CommandDeleteHostLocator = RegisterContainerLocator<CommandModel<ReactiveCommand, String>>(nameof(CommandDeleteHost), model => model.Initialize(nameof(CommandDeleteHost), ref model._CommandDeleteHost, ref _CommandDeleteHostLocator, _CommandDeleteHostDefaultValueFactory));
        static Func<BindableBase, CommandModel<ReactiveCommand, String>> _CommandDeleteHostDefaultValueFactory =
            model =>
            {
                var resource = nameof(CommandDeleteHost);           // Command resource  
                var commandId = nameof(CommandDeleteHost);
                var vm = CastToCurrentType(model);
                var cmd = new ReactiveCommand(canExecute: true) { ViewModel = model }; //New Command Core

                cmd.DoExecuteUIBusyTask(
                        vm,
                        async e =>
                        {

                            if (vm?.CurrentTask != null)
                            {
                                if (vm.CurrentTask.IsHosting)
                                {
                                    vm.CurrentTask.Stop();
                                }
                                vm.HostingTasks.Remove(vm.CurrentTask);
                            }

                            await MVVMSidekick.Utilities.TaskExHelper.Yield();
                        })
                    .DoNotifyDefaultEventRouter(vm, commandId)
                    .Subscribe()
                    .DisposeWith(vm);

                var cmdmdl = cmd.CreateCommandModel(resource);

                cmd.ListenCanExecuteObservable(
                    vm.ListenChanged(x => x.CurrentTask)
                    .Select(x => vm.CurrentTask != null)
                    );
                return cmdmdl;
            };

        #endregion


        public CommandModel<ReactiveCommand, String> CommandShowQR
        {
            get { return _CommandShowQRLocator(this).Value; }
            set { _CommandShowQRLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property CommandModel<ReactiveCommand, String> CommandShowQR Setup        

        protected Property<CommandModel<ReactiveCommand, String>> _CommandShowQR = new Property<CommandModel<ReactiveCommand, String>> { LocatorFunc = _CommandShowQRLocator };
        static Func<BindableBase, ValueContainer<CommandModel<ReactiveCommand, String>>> _CommandShowQRLocator = RegisterContainerLocator<CommandModel<ReactiveCommand, String>>(nameof(CommandShowQR), model => model.Initialize(nameof(CommandShowQR), ref model._CommandShowQR, ref _CommandShowQRLocator, _CommandShowQRDefaultValueFactory));
        static Func<BindableBase, CommandModel<ReactiveCommand, String>> _CommandShowQRDefaultValueFactory =
            model =>
            {
                var resource = nameof(CommandShowQR);           // Command resource  
                var commandId = nameof(CommandShowQR);
                var vm = CastToCurrentType(model);
                var cmd = new ReactiveCommand(canExecute: true) { ViewModel = model }; //New Command Core

                cmd.DoExecuteUIBusyTask(
                        vm,
                        async e =>
                        {
                            var v2 = ServiceLocator.Instance.Resolve<UriAndQRs_Model>();
                            v2.CurrentTask = vm.CurrentTask;
                            await vm.StageManager.DefaultStage.Show(v2);
                        })
                    .DoNotifyDefaultEventRouter(vm, commandId)
                    .Subscribe()
                    .DisposeWith(vm);

                var cmdmdl = cmd.CreateCommandModel(resource);

                cmd.ListenCanExecuteObservable(
                  vm.ListenChanged(x => x.CurrentTask)
                  .Select(x => vm.CurrentTask != null)
                  );
                return cmdmdl;
            };

        #endregion


        public CommandModel<ReactiveCommand, String> CommandSaveSettings
        {
            get { return _CommandSaveSettingsLocator(this).Value; }
            set { _CommandSaveSettingsLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property CommandModel<ReactiveCommand, String> CommandSaveSettings Setup        

        protected Property<CommandModel<ReactiveCommand, String>> _CommandSaveSettings = new Property<CommandModel<ReactiveCommand, String>> { LocatorFunc = _CommandSaveSettingsLocator };
        static Func<BindableBase, ValueContainer<CommandModel<ReactiveCommand, String>>> _CommandSaveSettingsLocator = RegisterContainerLocator<CommandModel<ReactiveCommand, String>>(nameof(CommandSaveSettings), model => model.Initialize(nameof(CommandSaveSettings), ref model._CommandSaveSettings, ref _CommandSaveSettingsLocator, _CommandSaveSettingsDefaultValueFactory));
        static Func<BindableBase, CommandModel<ReactiveCommand, String>> _CommandSaveSettingsDefaultValueFactory =
            model =>
            {
                var resource = nameof(CommandSaveSettings);           // Command resource  
                var commandId = nameof(CommandSaveSettings);
                var vm = CastToCurrentType(model);
                var cmd = new ReactiveCommand(canExecute: true) { ViewModel = model }; //New Command Core

                cmd.DoExecuteUIBusyTask(
                        vm,
                        async e =>
                        {
                            var store = ServiceLocator.Instance.Resolve<ISettingRepoService<ObservableCollection<ShareFileTask>>>();
                            await store.SaveAsync(vm.HostingTasks);
                            vm.GlobalEventRouter.RaiseEvent(vm, "Save Setting Successed.", "Logging");
                        })
                    .DoNotifyDefaultEventRouter(vm, commandId)
                    .Subscribe()
                    .DisposeWith(vm);

                var cmdmdl = cmd.CreateCommandModel(resource);

                cmdmdl.ListenToIsUIBusy(
                    model: vm,
                    canExecuteWhenBusy: false);
                return cmdmdl;
            };

        #endregion


        protected override async Task OnBindedViewLoad(IView view)
        {



            HostingTasks = await ExecuteTask(async () =>
              {
                  try
                  {
                      var store = ServiceLocator.Instance.Resolve<ISettingRepoService<ObservableCollection<ShareFileTask>>>();
                      return await store.LoadAsync();
                  }
                  catch (Exception)
                  {

                  }
                  return null;
              });

            if ((HostingTasks?.Count ?? 0) == 0)
            {
                var st = new ShareFileTask();
                HostingTasks = new ObservableCollection<Models.ShareFileTask> { st };
                CurrentTask = st;
                var f = ServiceLocator.Instance.Resolve<IFileSystemHubService>();
                CurrentTask.Path = await f.GetDefaultFolderAsync();


            }



            var t = ExecuteTask(async () =>
              {
                  await Task.Delay(500);
                  var cmd2 = CurrentTask.CommandStartHosting;
                  cmd2.Execute(null);
              });


            await base.OnBindedViewLoad(view);
        }



    }
}

