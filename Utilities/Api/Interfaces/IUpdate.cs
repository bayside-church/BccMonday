using System;
using System.Collections.Generic;

namespace com.baysideonline.BccMonday.Utilities.Api
{
    public interface IUpdate
    {
        long Id { get; set; }

        string Body { get; set; }

        string TextBody { get; set; }

        DateTime CreatedAt { get; set; }

        string CreatorName { get; }

        string CreatorId { get; set; }

        List<IFile> Files { get; set; }

        List<IUpdate> Replies { get; set; }

        MondayCreator Creator { get; set; }
    }
}
