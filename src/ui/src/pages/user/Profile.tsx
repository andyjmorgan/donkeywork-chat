import React, { useContext, useEffect, useState } from 'react';
import { Card } from 'primereact/card';
import { AuthContext } from '../../context/AuthContext';
import { GetUserInformationResponse } from '../../models/api/UserService/GetUserInformationResponse';
import { Skeleton } from 'primereact/skeleton';
import userService from '../../services/api/userService';
import '../../styles/components/Profile.css';

const Profile: React.FC = () => {
  const { user } = useContext(AuthContext);
  const [userProfile, setUserProfile] = useState<GetUserInformationResponse | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchUserProfile = async () => {
      try {
        setLoading(true);
        // Use the existing user data from context or fetch fresh data
        if (user) {
          setUserProfile(user);
        } else {
          const profileData = await userService.getUserInfo();
          setUserProfile(profileData);
        }
      } catch (error) {
        console.error('Error fetching user profile:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchUserProfile();
  }, [user]);


  return (
    <div className="profile-page p-4 grid">
      <div className="col-12">
        <Card className="mb-3">
          <div className="text-center mb-2">
            <h2 className="text-3xl font-bold mb-2 text-primary">User Profile</h2>
            <p className="text-lg">
              View and manage your account information
            </p>
          </div>
        </Card>
      </div>
      
      <div className="col-12">
        {loading ? (
          <Card className="shadow-2">
            <div className="p-3">
              <Skeleton width="200px" height="2rem" className="mb-3" />
              <div className="grid">
                <div className="col-12 md:col-6">
                  <Skeleton width="75%" height="1.5rem" className="mb-2" />
                  <Skeleton width="90%" height="1.2rem" className="mb-4" />
                </div>
                <div className="col-12 md:col-6">
                  <Skeleton width="75%" height="1.5rem" className="mb-2" />
                  <Skeleton width="90%" height="1.2rem" className="mb-4" />
                </div>
              </div>
              <Skeleton width="100%" height="1.5rem" className="mb-2" />
              <Skeleton width="75%" height="1.2rem" className="mb-4" />
              
              <Skeleton width="200px" height="2rem" className="mb-3" />
              <div className="grid">
                <div className="col-12 md:col-6">
                  <Skeleton width="75%" height="1.5rem" className="mb-2" />
                  <Skeleton width="90%" height="1.2rem" />
                </div>
                <div className="col-12 md:col-6">
                  <Skeleton width="75%" height="1.5rem" className="mb-2" />
                  <Skeleton width="90%" height="1.2rem" />
                </div>
              </div>
            </div>
          </Card>
        ) : (
          <Card className="shadow-2">
            <div className="p-3 mb-4">
              <h3 className="text-xl font-medium mb-3 pb-2 border-bottom-1 border-300">
                Account Information
              </h3>
              
              <div className="grid">
                <div className="col-12 md:col-6">
                  <div className="profile-field">
                    <div className="profile-field-label">User ID</div>
                    <div className="profile-field-value">{userProfile?.id || 'Not available'}</div>
                  </div>
                </div>
                
                <div className="col-12 md:col-6">
                  <div className="profile-field">
                    <div className="profile-field-label">Username</div>
                    <div className="profile-field-value">{userProfile?.userName || 'Not available'}</div>
                  </div>
                </div>
              </div>
              
              <div className="profile-field">
                <div className="profile-field-label">Email Address</div>
                <div className="profile-field-value">{userProfile?.emailAddress || 'Not available'}</div>
              </div>
            </div>
            
            <div className="p-4">
              <h3 className="text-xl font-medium mb-3 pb-2 border-bottom-1 border-300">
                Personal Information
              </h3>
              
              <div className="grid">
                <div className="col-12 md:col-6">
                  <div className="profile-field">
                    <div className="profile-field-label">First Name</div>
                    <div className="profile-field-value">{userProfile?.firstName || 'Not available'}</div>
                  </div>
                </div>
                
                <div className="col-12 md:col-6">
                  <div className="profile-field">
                    <div className="profile-field-label">Last Name</div>
                    <div className="profile-field-value">{userProfile?.familyName || 'Not available'}</div>
                  </div>
                </div>
              </div>
            </div>
          </Card>
        )}
      </div>
    </div>
  );
};

export default Profile;