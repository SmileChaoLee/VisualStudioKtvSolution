using System;
namespace VodManageSystem.Models
{
    public class ErrorCodeModel
    {
        public const int Succeeded = 1;
        public const int ErrorBecauseBugs = -1;

        public const int SongNotChanged = 0;
        public const int SongIsNull = -2;
        public const int SongNoIsEmpty = -3;
        public const int SongNoDuplicate = -4;
        public const int SongNoIsNotFound = -5;
        public const int Singer1NoNotFound = -7;
        public const int Singer2NoNotFound = -8;
        public const int OriginalSongNoIsEmpty = -11;
        public const int OriginalSongNotExist = -12;
        public const int VodNoOfSongIsEmpty = -21;
        public const int PathnameOfVodNoIsEmpty = -22;
        public const int Singer1IdIsNotSpecified = -23;
        public const int Singer2IdIsNotSpecified = -24;
        public const int NumOfWordsLessOrEqualToZero = -25;

        public const int LanguageNotChanged = -30;
        public const int LanguageIsNull = -32;
        public const int LanguageNoIsEmpty = -33;
        public const int LanguageNoDuplicate = -34;
        public const int LanguageNoNotFound = -35;
        public const int OriginalLanguageNoIsEmpty = -36;
        public const int OriginalLanguageNotExist = -37;
        public const int LanguageIdIsSpecified = -38;

        public const int SingareaNotChanged = -40;
        public const int SingareaIsNull = -42;
        public const int SingareaNoIsEmpty = -43;
        public const int SingareaNoDuplicate = -44;
        public const int SingareaNoNotFound = -45;
        public const int OriginalSingareaNoIsEmpty = -46;
        public const int OriginalSingareaNotExist = -47;

        public const int SingerNotChanged = -50;
        public const int SingerIsNull = -52;
        public const int SingerNoIsEmpty = -53;
        public const int SingerNoDuplicate = -54;
        public const int SingerNoNotFound = -55;
        public const int OriginalSingerNoIsEmpty = -56;
        public const int OriginalSingerNotExist = -57;

        public const int PlayerscoreNotChanged = -100;
        public const int PlayerscoreIsNull = -102;
        public const int PlayerNameIsEmpty = -103;
        public const int OriginalPlayerscoreNotExist = -107;

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
                case Singer1IdIsNotSpecified:
                    errorMsg = "Id for first singer is not specified.";
                    break;
                case Singer2IdIsNotSpecified:
                    errorMsg = "Id for second singer is not specified.";
                    break;
                case LanguageNotChanged:
                    errorMsg = "Language was unchanged.";
                    break;
                case LanguageIsNull:
                    errorMsg = "The object of Language was null.";
                    break;
                case LanguageNoIsEmpty:
                    errorMsg = "Language No. was empty or null.";
                    break;
                case LanguageIdIsSpecified:
                    errorMsg = "Language Id. was empty or null.";
                    break;
                case LanguageNoNotFound:
                    errorMsg = "Language No. was found.";
                    break;
                case LanguageNoDuplicate:
                    errorMsg = "Language No. was duplicate.";
                    break;
                case OriginalLanguageNoIsEmpty:
                    errorMsg = "The original language no was empty or null.";
                    break;
                case OriginalLanguageNotExist:
                    errorMsg = "The original language is no logner exist.";
                    break;
                case SingareaNotChanged:
                    errorMsg = "Singarea was unchanged.";
                    break;
                case SingareaIsNull:
                    errorMsg = "The object of Singarea was null.";
                    break;
                case SingareaNoIsEmpty:
                    errorMsg = "Singarea No. was empty or noll.";
                    break;
                case SingareaNoNotFound:
                    errorMsg = "Singarea No. was found.";
                    break;
                case SingareaNoDuplicate:
                    errorMsg = "Singarea No. was duplicate.";
                    break;
                case OriginalSingareaNoIsEmpty:
                    errorMsg = "The original singarea no was empty or null.";
                    break;
                case OriginalSingareaNotExist:
                    errorMsg = "The original singarea is no logner exist.";
                    break;
                case SingerNotChanged:
                    errorMsg = "Singer was unchanged.";
                    break;
                case SingerIsNull:
                    errorMsg = "The object of Singer was null.";
                    break;
                case SingerNoIsEmpty:
                    errorMsg = "Singer No. was empty or noll.";
                    break;
                case SingerNoNotFound:
                    errorMsg = "Singer No. was found.";
                    break;
                case SingerNoDuplicate:
                    errorMsg = "Singer No. was duplicate.";
                    break;
                case OriginalSingerNoIsEmpty:
                    errorMsg = "The original singer no was empty or null.";
                    break;
                case OriginalSingerNotExist:
                    errorMsg = "The original singer is no logner exist.";
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
