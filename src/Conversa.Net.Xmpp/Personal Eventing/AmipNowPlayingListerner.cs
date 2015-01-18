// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the New BSD License (BSD). See LICENSE file in the project root for full license information.

using Conversa.Net.Xmpp.Client;
using DJMatty.AMIP.ClientWrapper;
using System;

namespace Conversa.Net.Xmpp.PersonalEventing
{
#warning TODO: Implement as a background task
    public sealed class AmipNowPlayingListerner
    {
        private XmppClient client;
        private AMIPClient amip;
        //private Thread	   workerThread;

        internal AmipNowPlayingListerner(XmppClient client)
        {
            this.client = client;
        }

        public void Start()
        {
            this.Stop();

            //this.workerThread               = new Thread(new ThreadStart(Listen));
            //this.workerThread.IsBackground  = true;
            //this.workerThread.Start();
        }

        public void Stop()
        {
            //if (this.workerThread != null)
            //{
            //    this.workerThread.Interrupt();
            //    this.workerThread.Join();
            //    this.workerThread = null;
            //}
        }

        private void Listen()
        {
            string lastSong = null;

            try
            {
                // using will force dispose to be called -- dispose will ensure that the AMIP SDK is uninitialized
                // and any server listeners are removed
                using (this.amip = new AMIPClient("127.0.0.1", 60333, 5000, 5, 1, true))
                {
                    while (true)
                    {
                        /* var_s		: Song
                         * var_sl		: Song length
                         * var_psec		: Position
                         * var_br		: Bit rate
                         * var_sr		: Sample rate
                         * var_typ		: Song mode e.g. Stereo or Mono
                         * var_channels	:	1 for Mono, 2 for Stereo
                         * var_stat		: Status
                         * 		0: Stopped
                         * 		3: Paused
                         * 		1: Playing
                         * 		other: Unknown
                         * var_fn		: Full file name with extension and path
                         * var_1		: Artist
                         * var_2		: Title
                         * var_4		: Album
                         * var_5		: Year
                         * var_6		: Comment
                         * var_7		: Genre
                         */

                        string currentSong = null;
                        int    status      = 0;

                        try
                        {
                            status		= Convert.ToInt32(this.amip.Eval("var_stat"));
                            currentSong = this.amip.Eval("var_s");
                        }
                        catch (AMIPException)
                        {
                        }

                        if (status == 1 || status == 3)
                        {
                            if (!String.IsNullOrEmpty(currentSong) && (lastSong == null || lastSong != currentSong))
                            {
                                if (this.client.PersonalEventing.SupportsUserTune)
                                {
                                    string title = this.amip.Eval("var_4");

                                    var tune = new XmppUserTuneEvent
                                    (
                                        this.amip.Eval("var_1")
                                      , (ushort)(Convert.ToUInt16(this.amip.Eval("var_sl")) * 1000)
                                      , null
                                      , ((!String.IsNullOrEmpty(title)) ? title : currentSong)
                                      , this.amip.Eval("var_2")
                                      , null
                                      , null
                                    );

                                    //this.client.PublishTune(tune);

                                    lastSong = currentSong;
                                }
                            }
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(lastSong))
                            {
                                // If something has been played
                                //this.client.StopTunePublication();

                                lastSong = null;
                            }
                        }

                        // Wait 10 seconds before continue checking if something is being playing
                        // Thread.Sleep(10000);
                    }
                };
            }
            catch (Exception)
            {
            }

            this.client = null;
        }
    }
}
