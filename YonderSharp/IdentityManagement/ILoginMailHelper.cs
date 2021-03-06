﻿using System;

namespace YonderSharp.IdentityManagement
{
    /// <summary>
    /// Methods for sending mails
    /// </summary>
    public interface ILoginMailHelper
    {
        /// <summary>
        /// The user forgot his password. Send him a mail that resets the password
        /// </summary>
        public void SendPasswordForgottenMail(User user);

        /// <summary>
        /// Sends the mail that allows the verification of the ownership of the user of the used mail
        /// </summary>
        void SendRegistrationVerification(User user);

    }
}
