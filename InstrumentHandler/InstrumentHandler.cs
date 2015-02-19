﻿using InstrumentAbstractionModel;
using NationalInstruments.VisaNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentHandlerNamespace
{
    public sealed class InstrumentHandler
    {
        #region SingletoneStuff
        private static volatile InstrumentHandler m_Handler;
        private static object syncRoot = new object();
        private InstrumentHandler()
        {
            InitializeHandler();
        }

        
        

        public static InstrumentHandler Instance
        {
            get
            {
                lock (syncRoot)
                {
                    if (m_Handler == null)
                        m_Handler = new InstrumentHandler();
                }
                return m_Handler;
            }
        }

        #endregion

        private void InitializeHandler()
        {
            throw new NotImplementedException();
        }
        //
        //Write here device addresses statically.
        //

        private const string ResourceFilter = "GPIB|USB?*INSTR";
        private void DiscoverInstruments()
        {
            try
            {
                var LocalResourceManager = ResourceManager.GetLocalManager();
                var resources = LocalResourceManager.FindResources(ResourceFilter);
                foreach (var resource in resources)
                {
                    
                }
            }
            catch (VisaException)
            {
                
                throw;
            }
            catch(Exception ex)
            {
                throw;
            }
            
        }

        public bool TryGetDevice(IInstrumentOwner Owner, AvailableInstrumentsEmuneration InstrumentName, out IInstrument Instrument)
        {
            throw new NotImplementedException();
        }
                
        public bool TryGetDevices(IInstrumentOwner Owner, AvailableInstrumentsEmuneration InstrumentNames, out IInstrument[] Instruments)
        {
            throw new NotImplementedException();
        }
    }
}
