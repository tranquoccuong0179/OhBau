using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Entity;
using OhBau.Model.Payload.Response.Comment;

namespace OhBau.Model.Utils
{
    public static class CommentTreeUtil
    {
        public static List<GetComments> BuildCommentTree(IList<Comments> comments)
        {
            var commentList = comments.ToList();

            var commentLookup = commentList.ToLookup(c => c.ParentId);

            var commentTrees = new List<GetComments>();

            foreach (var comment in commentList.Where(c => c.ParentId == null))
            {
                commentTrees.Add(BuildCommentBranch(comment, commentLookup));
            }

            return commentTrees;
        }

        public static GetComments BuildCommentBranch(Comments comment, ILookup<Guid?, Comments> commentLookup)
        {
            var commentTree = new GetComments
            {
                Id = comment.Id,
                Comment = comment.Comment,
                Email = comment.Account?.Email!, 
                CreatedDate = comment.CreatedDate,
                UpdatedDate = comment.UpdatedDate
            };

            var replies = commentLookup[comment.Id];
            foreach (var reply in replies)
            {
                commentTree.Replies.Add(BuildCommentBranch(reply, commentLookup));
            }

            return commentTree;
        }

    }
}
