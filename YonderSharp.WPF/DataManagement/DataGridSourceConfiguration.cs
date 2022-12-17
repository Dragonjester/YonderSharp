using System.Runtime.Serialization;

namespace YonderSharp.WPF.DataManagement
{
    /// <summary>
    /// Configuration for a <see cref="IDataGridSource"/>
    /// </summary>
    [DataContract]
    public class DataGridSourceConfiguration
    {
        [DataMember]
        public bool IsAllowedToIsAllowedToAddFromList { get; set; }

        [DataMember]
        public bool IsAllowedToCreateNewEntry { get; set; }

        [DataMember]
        public bool IsAllowedToRemove { get; set; }

        [DataMember]
        public bool HasSearch { get; set; }

        /// <summary>
        /// Should the textbox for the primary field key be disabled?
        /// </summary>
        [DataMember]
        public bool IsPrimaryKeyDisabled { get; set; }

        /// <summary>
        /// should the GetAddableItems return all items or only those that aren't in the given list.
        /// True = return all
        /// </summary>
        [DataMember]
        public bool GetAddableItemsReturnAll { get; set; }

        /// <summary>
        /// Show the save dialog on successfull save?
        /// </summary>
        [DataMember]
        public bool ShowSaveDialog { get; set; }

        /// <summary>
        /// Disables all controlls
        /// </summary>
        [DataMember]
        public bool IsReadOnlyMode { get; set; }

        /// <summary>
        /// Controlls wether the Save button is shown
        /// </summary>
        [DataMember]
        public bool ShowSaveButton { get; set; }
    }
}
