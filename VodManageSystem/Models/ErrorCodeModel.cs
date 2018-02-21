using System;
namespace VodManageSystem.Models
{
    public class ErrorCodeModel
    {
        public const int SongNotChanged = 0;
        public const int Succeeded = 1;
        public const int ErrorBecauseBugs = -1;
        public const int SongIsNull = -2;
        public const int SongNoIsEmpty = -3;
        public const int SongNoDuplicate = -4;
        public const int LangeNoIsEmpty = -5;
        public const int LangeNoNotFound = -6;
        public const int Singer1NoNotFound = -7;
        public const int Singer2NoNotFound = -8;
        public const int OriginalSongNoIsEmpty = -11;
        public const int OriginalSongNotExist = -12;
        public const int VodNoOfSongIsEmpty = -21;
        public const int PathnameOfVodNoIsEmpty = -22;
        public const int DatabaseError = -99;
        public const int ModelBindingFailed = -999;

        /// <summary>
        /// empty constructor
        /// </summary>
        public ErrorCodeModel()
        {
        }

        /// <summary>
        /// Return the error message depends on error code
        /// </summary>
        /// <returns>Error message.</returns>
        /// <param name="errorCode">Error code.</param>
        public static string GetErrorMessage(int errorCode)
        {
            string errorMsg = "";

            switch(errorCode)
            {
                case SongNotChanged:
                    errorMsg = "Song was unchanged.";
                    break;
                case Succeeded:
                    errorMsg = "Succeeded to update song table.";
                    break;
                case ErrorBecauseBugs:
                    errorMsg = "There was a bug in codes. It could be Id = 0.";
                    break;
                case SongIsNull:
                    errorMsg = "The object of Song was null.";
                    break;
                case SongNoIsEmpty:
                    errorMsg = "Song No. was empty or null.";
                    break;
                case SongNoDuplicate:
                    errorMsg = "Song No. was duplicate.";
                    break;
                case LangeNoIsEmpty:
                    errorMsg = "Language No. was empty or noll.";
                    break;
                case LangeNoNotFound:
                    errorMsg = "Language No. was found.";
                    break;
                case Singer1NoNotFound:
                    errorMsg = "The singer No. for first singer was no found";
                    break;
                case Singer2NoNotFound:
                    errorMsg = "The singer No. for second singer was no found";
                    break;
                case OriginalSongNoIsEmpty:
                    errorMsg = "The original song no was empty or null.";
                    break;
                case OriginalSongNotExist:
                    errorMsg = "The original song is no logner exist.";
                    break;
                case VodNoOfSongIsEmpty:
                    errorMsg = "Vod No. for this song was empty.";
                    break;
                case PathnameOfVodNoIsEmpty:
                    errorMsg = "Path name for this Vod video was empty.";
                    break;
                case DatabaseError:
                    errorMsg = "Error on database exception.";
                    break;
                case ModelBindingFailed:
                    errorMsg = "Model binding failed (Model.IsValid = false.)";
                    break;
                default:
                    errorMsg = "Unknown error";
                    break;
                    
            }

            return errorMsg;
        }
    }
}
