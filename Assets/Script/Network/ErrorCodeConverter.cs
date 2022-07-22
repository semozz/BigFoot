using UnityEngine;
using System.Collections;
using Hive5;

/// <summary>
/// Error code converter.
/// Convert hive5 error code to BigFoot error code
/// </summary>
public static class ErrorCodeConverter
{

    public static NetErrorCode Convert(Hive5ResultCode resultCode, string resultMessage = "")
    {
        Debug.Log("In ErrorCodeConverter:" + resultCode);
        switch (resultCode)
        {

            case Hive5ResultCode.Success:
                return NetErrorCode.OK;

            case Hive5ResultCode.NetworkError:
                return NetErrorCode.NotConnected;

            case Hive5ResultCode.HttpStatusContinue:

                break;

            case Hive5ResultCode.HttpStatusSwitchingProtocal:

                break;

            case Hive5ResultCode.HttpStatusOK:

                break;

            case Hive5ResultCode.HttpStatusCreated:

                break;

            case Hive5ResultCode.HttpStatusAccepted:

                break;

            case Hive5ResultCode.HttpStatusNonAuthoritativeInformation:

                break;

            case Hive5ResultCode.HttpStatusNoContent:

                break;

            case Hive5ResultCode.HttpStatusResetContent:

                break;

            case Hive5ResultCode.HttpStatusPartialContent:

                break;

            case Hive5ResultCode.HttpStatusMultipleChoice:

                break;

            case Hive5ResultCode.HttpStatusMovedPermanently:

                break;

            case Hive5ResultCode.HttpStatusFound:

                break;

            case Hive5ResultCode.HttpStatusSeeOther:

                break;

            case Hive5ResultCode.HttpStatusNotMofified:

                break;

            case Hive5ResultCode.HttpStatusUseProxy:

                break;

            case Hive5ResultCode.HttpStatusUnused:

                break;

            case Hive5ResultCode.HttpStatusTemporaryRedirect:

                break;

            case Hive5ResultCode.HttpStatusPermanentRedirect:

                break;

            case Hive5ResultCode.HttpStatusBadRequest:

                break;

            case Hive5ResultCode.HttpStatusUnauthorized:

                break;

            case Hive5ResultCode.HttpStatusPaymentRequired:

                break;

            case Hive5ResultCode.HttpStatusForbidden:

                break;

            case Hive5ResultCode.HttpStatusNotFound:

                break;

            case Hive5ResultCode.HttpStatusMethodNotAllowed:

                break;

            case Hive5ResultCode.HttpStatusNotAcceptable:

                break;

            case Hive5ResultCode.HttpStatusProxyAuthenticationRequired:

                break;

            case Hive5ResultCode.HttpStatusRequestTimeout:
                return NetErrorCode.WebTimeOut;

            case Hive5ResultCode.HttpStatusConflict:

                break;

            case Hive5ResultCode.HttpStatusGone:

                break;

            case Hive5ResultCode.HttpStatusLengthRequired:

                break;

            case Hive5ResultCode.HttpStatusPreconditionFailed:

                break;

            case Hive5ResultCode.HttpStatusRequestEntityTooLarge:

                break;

            case Hive5ResultCode.HttpStatusRequestURITooLong:

                break;

            case Hive5ResultCode.HttpStatusUnsupportedMediaType:

                break;

            case Hive5ResultCode.HttpStatusRequestedRangeNotSatisfiable:

                break;

            case Hive5ResultCode.HttpStatusExpectationFailed:

                break;

            case Hive5ResultCode.HttpStatusInternalServerError:

                break;

            case Hive5ResultCode.HttpStatusNotImplemented:

                break;

            case Hive5ResultCode.HttpStatusBadGateway:

                break;

            case Hive5ResultCode.HttpStatusServiceUnavailable:

                break;

            case Hive5ResultCode.HttpStatusGatewayTimeout:
                return NetErrorCode.WebTimeOut;

            case Hive5ResultCode.HttpStatusHttpVersionNotSupported:

                break;

            case Hive5ResultCode.InvalidParameter:
                return NetErrorCode.PacketCodeInvalid;

            case Hive5ResultCode.DataDoesNotExist:

                break;

            case Hive5ResultCode.InvalidReward:

                break;

            case Hive5ResultCode.InvalidPurchaseStatus:

                break;

            case Hive5ResultCode.InvalidPaymentSequence:

                break;

            case Hive5ResultCode.InvalidAppleReceipt:

                break;

            case Hive5ResultCode.InvalidGooglePurchaseData:

                break;

            case Hive5ResultCode.InvalidGoogleSignature:

                break;

            case Hive5ResultCode.NoGoogleIapPublicKeyIsRegistered:

                break;

            case Hive5ResultCode.InvalidGoogleIapPublicKey:

                break;

                //case Hive5ResultCode.NoKakaoAppAuthInfo:

                break;

            case Hive5ResultCode.NoIapConversion:

                break;

            case Hive5ResultCode.MissionAlreadyCompleted:

                break;

            //                case Hive5ResultCode.PromotionCodeAlreadyConsumed:
            //
            //                    break;
            //
            //                case Hive5ResultCode.InvalidPromotionCode:
            //
            //                    break;

            case Hive5ResultCode.NotFriend:

                break;

            case Hive5ResultCode.TheItemIsNotAbleToGift:

                break;

            case Hive5ResultCode.TheItemCannotGiftBecauseItHasRecentlyGifted:

                break;

            case Hive5ResultCode.TooManyCountToGift:

                break;

            case Hive5ResultCode.AlreadyExistingNickname:
                return NetErrorCode.NickNameDuplicate;

            case Hive5ResultCode.ForbiddenNickname:
                return NetErrorCode.NickNameInvalid;

            case Hive5ResultCode.JavascriptExceptionOnProcedure:
                return NetErrorCode.ExceptionOnProcedure;

            case Hive5ResultCode.UndefinedProcedure:

                break;

            case Hive5ResultCode.ProtectedProcedure:

                break;

            case Hive5ResultCode.ProtectedMethodClassDescriptor:

                break;

            case Hive5ResultCode.UndefinedLibrary:

                break;

            case Hive5ResultCode.ObjectNoFound:

                break;

            case Hive5ResultCode.SingletonCanNotBeDestroyed:

                break;

            case Hive5ResultCode.InvalidObjectField:

                break;

            case Hive5ResultCode.TooManyObjectFields:

                break;

            case Hive5ResultCode.DataTableNotFound:

                break;

            case Hive5ResultCode.InvalidReturn:

                break;

            case Hive5ResultCode.UndefinedAppDataKey:

                break;

            case Hive5ResultCode.ExecutionTimeout:
                return NetErrorCode.WebTimeOut;
                

            case Hive5ResultCode.StackOverflow:

                break;

            case Hive5ResultCode.UnsupportedLibraryOrFunction:

                break;

            case Hive5ResultCode.UnsupportedDataType:

                break;

            case Hive5ResultCode.AlreadyExistingPlatformUserName:
            case Hive5ResultCode.AlreadyExistingPlatformUserEmail:
                return NetErrorCode.LoginAccount_Duplicate;

            case Hive5ResultCode.InvalidNameOrPassword:
                return NetErrorCode.LoginPassword_Wrong;

            //                case Hive5ResultCode.InvalidPayload:
            //
            //                    break;

            case Hive5ResultCode.TheUserHasBeenBlocked:
                break;

            //                case Hive5ResultCode.InvalidAppConfiguration:
            //
            //                    break;

            //                case Hive5ResultCode.InvalidServiceName:
            //
            //                    break;

            //                case Hive5ResultCode.InvalidJobItem:
            //
            //                    break;
            //
            //                case Hive5ResultCode.NotImplemented:
            //
            //                    break;
            case Hive5ResultCode.TheSessionKeyIsInvalid:
                return NetErrorCode.DuplicateConnection;

            case Hive5ResultCode.UnknownError:

                break;

            default:

                break;
        }

        // end_stage timeout message´Â 9999 ?
        if (resultMessage.IndexOf("Timeout of Controller Action") > 0)
            return NetErrorCode.WebTimeOut;

        return NetErrorCode.Unknown;
    }
}
