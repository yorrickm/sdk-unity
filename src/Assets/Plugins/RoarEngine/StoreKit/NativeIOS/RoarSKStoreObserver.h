#import <StoreKit/StoreKit.h>
#import <StoreKit/SKPaymentTransaction.h>
#import "RoarSKStore.h"

@class RoarSKStore;

// Based on the sample code in the official In-App Purchase Programming Guide.
// @see http://developer.apple.com/library/ios/#documentation/NetworkingInternet/Conceptual/StoreKitGuide/AddingaStoretoYourApplication/AddingaStoretoYourApplication.html
@interface RoarSKStoreObserver : NSObject<SKPaymentTransactionObserver>
{
//	RoarSKStore *roarSKStore;
}

@property(nonatomic, strong) RoarSKStore *roarSKStore;

- (RoarSKStoreObserver*)initWithRoarSKStore:(RoarSKStore *)store;
- (void)paymentQueue:(SKPaymentQueue *)queue updatedTransactions:(NSArray *)transactions;

@end
