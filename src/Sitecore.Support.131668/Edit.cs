using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Security.AccessControl;
using Sitecore.Shell.Framework.Commands;
using System;
namespace Sitecore.Support.Shell.Framework.Commands.ContentEditor.Edit
{
    [Serializable]
    public class Edit : Sitecore.Shell.Framework.Commands.ContentEditor.Edit
    {
        public override CommandState QueryState(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            if (context.Items.Length != 1)
            {
                return CommandState.Hidden;
            }
            Item item = context.Items[0];
            if (!base.HasField(item, FieldIDs.Workflow) || !base.HasField(item, FieldIDs.WorkflowState))
            {
                return CommandState.Hidden;
            }
            if (HasWriteAccess(item))
            {
                if (!item.Access.CanWriteLanguage())
                {
                    return CommandState.Disabled;
                }
                if (CanCheckIn(item))
                {
                    return CommandState.Down;
                }
                if (CanEdit(item))
                {
                    return CommandState.Enabled;
                }
            }
            return CommandState.Disabled;
        }
        protected static new bool CanEdit(Item item)
        {
            Assert.ArgumentNotNull(item, "item");
            if (item.Appearance.ReadOnly || !CanEditItem(item))
            {

                return false;
            }
            if (AuthorizationManager.IsAllowed(item, AccessRight.ItemWrite, Context.User))
            {
                return !item.Locking.HasLock();
            }
            return true;
        }

        private static bool CanEditItem(Item item)
        {
            if (Context.User.IsAdministrator)
            {
                return true;
            }

            if (!TemplateManager.IsFieldPartOfTemplate(FieldIDs.Lock, item))
            {
                return true;
            }
            if (item.Locking.CanLock())
            {
                return true;
            }

            return false;
        }


    }
}