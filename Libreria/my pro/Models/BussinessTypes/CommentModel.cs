using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace my_pro.Models.BussinessTypes
{
    public class CommentModel :Comment
    {
        public UserAccount member { get; set; }

        public BookModel book { get; set; }
        public static List<CommentModel> GetCommentsByBookID (long bookID)
        {
            using (onlinebookstore db = new onlinebookstore())
            {
                var c = db.Comments.Where(a => a.Fk_BookID == bookID).ToList();
                List<CommentModel> comments = new List<CommentModel>();
                foreach (var com in c)
                {
                    CommentModel cm = new CommentModel()
                    {
                        member = UserAccount.GetMemberShortInfoByID(com.Fk_memberID),
                        Date = com.Date,
                        Comment1 = com.Comment1,
                        Fk_memberID = com.Fk_memberID,
                        Fk_BookID = com.Fk_BookID,
                        Pk_CommentID = com.Pk_CommentID
                    };
                    comments.Add(cm);
                }
                return comments;
            }
        }

        public static List<CommentModel> GetCommentsBymemberID (long memberID)
        {
            using (onlinebookstore db = new onlinebookstore())
            {
                var c = db.Comments.Where(a => a.Fk_memberID == memberID).ToList();
                List<CommentModel> comments = new List<CommentModel>();
                foreach (var com in c)
                {
                    CommentModel cm = new CommentModel()
                    {
                        Date = com.Date,
                        Comment1 = com.Comment1,
                        Fk_memberID = com.Fk_memberID,
                        Fk_BookID = com.Fk_BookID,
                        Pk_CommentID = com.Pk_CommentID,
                        book = BookModel.GetBookInfos(com.Fk_BookID)
                    };
                    if (cm.book.book.isDeletes == null && cm.book.book.isDeletes !=true)
                    {
                        comments.Add(cm);
                    }
                }
                return comments;
            }

        }
    }
}