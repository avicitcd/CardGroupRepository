using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardWorkbench.test
{
    public class ChannelRegisterSetupSimTest
    {
        public static string getJsonStr() {
           string json = @"{
                               'McfsControlRegisters' : {
                                  'ControlRegister' : {
                                     'MCFS_DECODE' : 'McfsDecodeNrzl',
                                     'MCFS_INPUT_CLOCK_POLARITY' : 'McfsPolarity0Degree',
                                     'MCFS_INPUT_SOURCE' : 'McfsSimInput',
                                     'MCFS_MESSAGE_WORD_LENGTH' : 'McfsMessageWordlength16',
                                     'MCFS_WATCHDOG_TIMER' : 'McfsWatchDogDisable'
                                  },
                                  'FrameStrategyModeControlsRegister' : {
                                     'MCFS_BIT_SLIP_WINDOW' : 'McfsWindow1Bit',
                                     'MCFS_INPUT_POLARITY' : 'McfsPolarityNormal',
                                     'MCFS_SYNC_MODE' : 'McfsSyncModeFixed',
                                     'MCFS_SYNC_PATTERN_FORMAT' : 'McfsSyncPatternNormal',
                                     'MCFS_VARIABLE_LENGTH_FRAME_POSITION' : 'McfsRandomFramePosition',
                                     'McfsSyncPatternLength' : 16
                                  },
                                  'FrameSyncStrategyRegister' : {
                                     'McfsErrorToleranceCount' : 1,
                                     'McfsLockToSearchCount' : 1,
                                     'McfsVerifyToLockCount' : 1,
                                     'McfsVerifyToSearchCount' : 1
                                  },
                                  'SyncPatternRegisters' : {
                                     'McfsSyncMask1' : 'fff',
                                     'McfsSyncMask2' : '0',
                                     'McfsSyncMask3' : '0',
                                     'McfsSyncMask4' : '0',
                                     'McfsSyncPattern1' : 'fff',
                                     'McfsSyncPattern2' : '0',
                                     'McfsSyncPattern3' : '0',
                                     'McfsSyncPattern4' : '0'
                                  }
                               }
                           }";
           return json;
        }

        public static string getInitJsonStr()
        {
            string json = @"{
                               'McfsControlRegisters' : {
                                  'ControlRegister' : {
                                     'MCFS_DECODE' : 'McfsDecodeNrzs',
                                     'MCFS_INPUT_CLOCK_POLARITY' : 'McfsPolarity180Degree',
                                     'MCFS_INPUT_SOURCE' : 'McfsSimInput',
                                     'MCFS_MESSAGE_WORD_LENGTH' : 'McfsMessageWordlength16',
                                     'MCFS_WATCHDOG_TIMER' : 'McfsWatchDogDisable'
                                  },
                                  'FrameStrategyModeControlsRegister' : {
                                     'MCFS_BIT_SLIP_WINDOW' : 'McfsWindow1Bit',
                                     'MCFS_INPUT_POLARITY' : 'McfsPolarityNormal',
                                     'MCFS_SYNC_MODE' : 'McfsSyncModeFixed',
                                     'MCFS_SYNC_PATTERN_FORMAT' : 'McfsSyncPatternNormal',
                                     'MCFS_VARIABLE_LENGTH_FRAME_POSITION' : 'McfsRandomFramePosition',
                                     'McfsSyncPatternLength' : 32
                                  },
                                  'FrameSyncStrategyRegister' : {
                                     'McfsErrorToleranceCount' : 2,
                                     'McfsLockToSearchCount' : 2,
                                     'McfsVerifyToLockCount' : 2,
                                     'McfsVerifyToSearchCount' : 2
                                  },
                                  'SyncPatternRegisters' : {
                                     'McfsSyncMask1' : 65533,
                                     'McfsSyncMask2' : 0,
                                     'McfsSyncMask3' : 0,
                                     'McfsSyncMask4' : 0,
                                     'McfsSyncPattern1' : 60304,
                                     'McfsSyncPattern2' : 0,
                                     'McfsSyncPattern3' : 0,
                                     'McfsSyncPattern4' : 0
                                  }
                               }
                           }";
            return json;
        }
    }
    
}
