using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Godot;
using XCardGame.Common;
using XCardGame.Ui;

namespace XCardGame;

public partial class BattleLog: DialogueBox
{
    public class BattleLogInputHandler: BaseInputHandler
    {
        public BattleLog BattleLog;
        
        public BattleLogInputHandler(GameMgr gameMgr, BattleLog battleLog) : base(gameMgr)
        {
            BattleLog = battleLog;
        }

        protected override void OnLeftMouseButtonPressed(Vector2 position)
        {
            base.OnLeftMouseButtonPressed(position);
            BattleLog.Proceed();
        }
    }
    
    public GameMgr GameMgr;
    
    public Action OnLogProceed;
    public Action OnLogFinished;
    // public TaskCompletionSource LogFinished;

    public ObservableCollection<string> HistoryLogEntries;
    public List<string> LogEntriesToDisplay;

    // protected BattleLogInputHandler InputHandler;
    protected bool SuppressNotification;

    public override void _Ready()
    {
        base._Ready();
        GameMgr = GetNode<GameMgr>("/root/GameMgr");
        HistoryLogEntries = new ObservableCollection<string>();
        HistoryLogEntries.CollectionChanged += OnHistoryLogsChanged;
        LogEntriesToDisplay = new List<string>();
        SuppressNotification = false;
        // InputHandler = new BattleLogInputHandler(GameMgr, this);
        // LogFinished = new TaskCompletionSource();
    }

    // public void LogAppend(string logEntry)
    // {
    //     LogEntriesToDisplay.Add(logEntry);
    // }
    //
    // public void Log(List<string> logEntries)
    // {
    //     LogEntriesToDisplay.Clear();
    //     LogEntriesToDisplay.AddRange(logEntries);
    //     GameMgr.InputMgr.SwitchToInputHandler(InputHandler);
    // }

    public void Proceed()
    {
        if (LogEntriesToDisplay.Count > 0)
        {
            if (HistoryLogEntries.Count >= Configuration.MaxLogEntriesCount)
            {
                HistoryLogEntries.RemoveAt(0);
            }
            HistoryLogEntries.Add(LogEntriesToDisplay[0]);
            LogEntriesToDisplay.RemoveAt(0);
            OnLogProceed?.Invoke();
        }
        else
        {
            // GameMgr.InputMgr.QuitCurrentInputHandler();
            // LogFinished.SetResult();
            OnLogFinished?.Invoke();
        }
    }

    public void Log(List<string> logEntries)
    {
        LogEntriesToDisplay.AddRange(logEntries);
    }

    public void Log(string logEntry)
    {
        GD.Print($"BattleLog: {logEntry}");
        LogEntriesToDisplay.Add(logEntry);
    }
    
    public async Task LogAndWait(List<string> logEntries, float interval = 0f)
    {
        LogEntriesToDisplay.AddRange(logEntries);
        await HandleLogEntries(interval);
        // await LogFinished.Task;
    }
    
    public async Task LogAndWait(string logEntry, float interval = 0f) 
    {
        GD.Print($"BattleLog: {logEntry}");
        LogEntriesToDisplay.Add(logEntry);
        await HandleLogEntries(interval);
    }

    public async Task HandleLogEntries(float interval = 0f)
    {
        while (LogEntriesToDisplay.Count > 0)
        {
            Proceed();
            if (interval > 0)
            {
                await Utils.Wait(this, interval);
            }
        }
    }
    
    protected void OnHistoryLogsChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
        Content.Value = string.Join("\n", HistoryLogEntries);
    }
}