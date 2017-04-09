﻿using ExperimentDataModel;
using Instruments;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExperimentAbstraction
{
    public abstract class NewAbstractExperiment<InfoT, DataT> : ObservableExperiment<DataT>, INewExperiment, IDisposable
        where InfoT : struct,IMeasurementInfo
        where DataT : struct
    {

        public NewAbstractExperiment(string ExperimentName)
        {
            this.Name = ExperimentName;
            this.ExperimentName = ExperimentName;
            this.SimulateExperiment = false;
            this._cancellationSourceToken = new CancellationTokenSource();
            this._pauseSourceToken = new PauseTokenSource();
        }

        private CancellationTokenSource _cancellationSourceToken;
        private PauseTokenSource _pauseSourceToken;
        private StreamMeasurementDataExporter<InfoT, DataT> _dataWriter;

        protected abstract void InitializeWriter();
        public virtual void InitializeExperiment()
        {
            
        }
        public virtual void FinalizeExperiment()
        {

        }

        public abstract void InitializeInstruments();
        public abstract void OwnInstruments();
        public abstract void ReleaseInstruments();
        
        public bool SimulateExperiment
        {
            get;
            set;
        }
        public string Name { get; private set; }
        protected string WorkingDirectory { get; set; }
        protected string ExperimentName { get; set; }
        protected string MeasurementName { get; set; }
        protected int MeasurementCount { get; set; }
        public object ViewModel
        {
            get { throw new NotImplementedException(); }
        }


        protected virtual void AssertParams()
        {
            if (String.IsNullOrEmpty(WorkingDirectory))
                throw new ArgumentNullException("Working directory is not set");

            if (String.IsNullOrEmpty(ExperimentName))
                throw new ArgumentNullException("Experiment name is not set");

            if (String.IsNullOrEmpty(MeasurementName))
                throw new ArgumentNullException("MeasurementName is not set");

            if (MeasurementCount < 0)
                throw new ArgumentNullException("Measurement count is not set");
        }

        public bool Equals(IInstrumentOwner other)
        {
            if (other.Name == Name)
                if (Object.ReferenceEquals(this, other))
                    return true;
            return false;
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public void Pause()
        {
            _pauseSourceToken.IsPaused = true;
        }

        public void Resume()
        {
            _pauseSourceToken.IsPaused = false;
        }

        [Obsolete("Use Execute method with IProgress<ExecutionReport> progress parameter")]
        public void Execute()
        {
            throw new NotImplementedException();
        }
        [Obsolete("Use Execute method with IProgress<ExecutionReport> progress parameter")]
        public void Execute(object ExperimentStartObject, DoWorkEventArgs e)
        {
            InitializeExperiment();
            if (SimulateExperiment)
            {
                AssertParams();
                PerformSimulatedExperiment(ExperimentStartObject, e);
            }
            else
            {
                InitializeInstruments();
                OwnInstruments();
                AssertParams();
                PerformExperiment(ExperimentStartObject, e);
            }
        }

        public void Execute(IProgress<ExecutionReport> progress)
        {
            InitializeExperiment();
            AssertParams();
            OnExecutionStarted(this, new EventArgs());
            IsRunning = true;
            Status = ExecutionStatus.Running;
            OnStatusChanged(this, Status);
            try
            {
                if (SimulateExperiment)
                {
                    PerformSimulatedExperiment(progress, _cancellationSourceToken.Token,_pauseSourceToken.Token);
                }
                else
                {
                    InitializeInstruments();
                    OwnInstruments();
                    PerformExperiment(progress, _cancellationSourceToken.Token, _pauseSourceToken.Token);
                }
            }catch(OperationCanceledException e)
            {
                Status = ExecutionStatus.Aborted;
                HandleError(e);
                OnExecutionAborted(this, new EventArgs());
            }
            catch(Exception e)
            {
                Status = ExecutionStatus.Failed;
                HandleError(e);
                
            }
            finally
            {
                IsRunning = false;
                OnStatusChanged(this,Status);
                OnExecutionFinished(this, new EventArgs());
            }
        }

        protected abstract void PerformExperiment(object ExperimentStartObject, DoWorkEventArgs e);
        protected abstract void PerformExperiment(IProgress<ExecutionReport> progress, CancellationToken cancellationToken, PauseToken pauseToken);
        protected abstract void PerformSimulatedExperiment(object ExperimentStartObject, DoWorkEventArgs e);
        protected abstract void PerformSimulatedExperiment(IProgress<ExecutionReport> progress, CancellationToken cancellationToken, PauseToken pauseToken);
              
        public void Abort()
        {
            if (_cancellationSourceToken != null)
                _cancellationSourceToken.Cancel();
        }

        public bool IsRunning
        {
            get;
            private set;
        }

        public ExecutionStatus Status
        {
            get;
            private set;
        }

        protected abstract void HandleError(Exception e);
        protected abstract void HandleMessage(string Message);

        #region events
        public event EventHandler<ExecutionStatus> StatusChanged;
        private void OnStatusChanged(object sender, ExecutionStatus status)
        {
            var handler = StatusChanged;
            if(handler != null)
            {
                handler(sender, status);
            }
        }

        public event EventHandler ExecutionStarted;

        protected virtual void OnExecutionStarted(object sender, EventArgs e)
        {
            var handler = ExecutionStarted;
            if (handler != null)
                handler(sender, e);
        }

        public event EventHandler ExecutionAborted;
        protected virtual void OnExecutionAborted(object sender, EventArgs e)
        {
            var handler = ExecutionAborted;
            if (handler != null)
                handler(sender, e);
        }


        public event ProgressChangedEventHandler ProgressChanged;
        protected virtual void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var handler = ProgressChanged;
            if (handler != null)
                handler(sender, e);
        }

        public event EventHandler ExecutionFinished;
        protected virtual void OnExecutionFinished(object sender, EventArgs e)
        {
            var handler = ExecutionFinished;
            if (handler != null)
                handler(sender, e);
        }
        #endregion

        #region disposal
        public void Dispose()
        {
            throw new NotImplementedException();
        }
        #endregion





       
    }
}