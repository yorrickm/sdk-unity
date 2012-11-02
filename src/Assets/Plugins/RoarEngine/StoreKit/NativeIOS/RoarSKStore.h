#import <StoreKit/StoreKit.h>
#import <StoreKit/SKProductsRequest.h>
#import "RoarSKStoreObserver.h"

void UnitySendMessage(const char* obj, const char* method, const char* msg);

@class RoarSKStoreObserver;

// Based on the sample code in the official In-App Purchase Programming Guide.
// @see http://developer.apple.com/library/ios/#documentation/NetworkingInternet/Conceptual/StoreKitGuide/AddingaStoretoYourApplication/AddingaStoretoYourApplication.html
@interface RoarSKStore : NSObject<SKProductsRequestDelegate>
{
	NSString *unityListenerName;
    RoarSKStoreObserver *observer;
}

@property(nonatomic, retain) NSString *unityListenerName;
@property(nonatomic, strong) RoarSKStoreObserver *observer;

- (RoarSKStore*)init:(const char*) listenerGameObject;
- (void)setUnityListener:(const char*) listenerGameObject;

- (bool)purchasesEnabled;
- (void)requestProductData:(const char*) cProductIdentifiers;
- (void)productsRequest:(SKProductsRequest *)request didReceiveResponse:(SKProductsResponse *)response;

- (void)purchaseProduct:(const char*) cProductIdentifier;
- (void)purchaseProduct:(const char*) cProductIdentifier withQuantity:(int)quantity;

- (void)onCompleteTransaction:(SKPaymentTransaction *)transaction forProduct:(NSString *)productIdentifier;
- (void)onFailedTransaction:(SKPaymentTransaction *)transaction forProduct:(NSString *)productIdentifier;

@end
